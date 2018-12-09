import { Component, OnInit } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { NgForm, FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { LotteryService } from '../../services/lotteryservice.service';
import { forEach } from '@angular/router/src/utils/collection';
import { Console } from '@angular/core/src/console';

@Component({
    templateUrl: './aposta.component.html'
})

export class ApostaComponent implements OnInit {
    apostaForm: FormGroup;
    nameCtrl: FormControl;
    apostaCtrl: FormControl;
    errorMessage: string = '';
    isAutomatic: boolean = false;
    gameType: number = 1;
    public apostaList: Apostas[] = [];

    constructor(private formBuilder: FormBuilder, private _lotteryService: LotteryService)
    {
        this.nameCtrl = this.formBuilder.control('');
        this.apostaCtrl = this.formBuilder.control('');

        this.apostaForm = this.formBuilder.group({
            name: this.nameCtrl,
            combinations: this.apostaCtrl,
            gameType: this.gameType,
            automatic: this.isAutomatic
        });
    }

    ngOnInit() {

        this._lotteryService.getGamesList().subscribe(data => {
            this.apostaList = data;
        })
    }

    toggle() {
        this.isAutomatic = !this.isAutomatic;
    }

    save() {
        this.errorMessage = '';

        if (!this.apostaForm.valid) {
            return;
        }

        this.apostaForm.controls['automatic'].setValue(this.isAutomatic);

        this._lotteryService.saveAposta(this.apostaForm.value)
            .subscribe(data => {
                if (data.error === '') {
                    this.apostaList = data.listApostas
                    this.reset();
                }
                else {
                    this.errorMessage = data.error.split(',');
                }
            })
    }

    reset() {
        this.apostaCtrl.setValue('');
        this.nameCtrl.setValue('');
        this.errorMessage = '';
    }
}  

interface Apostas {
    Id: number;
    Name: string;
    Combinations: string;
    LotteryType: number;
    TimeStamp: number;
}