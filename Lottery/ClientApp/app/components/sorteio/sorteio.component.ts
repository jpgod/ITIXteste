import { Component, OnInit } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { NgForm, FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { LotteryService } from '../../services/lotteryservice.service';
import { forEach } from '@angular/router/src/utils/collection';

@Component({
    templateUrl: './sorteio.component.html'
})

export class SorteioComponent implements OnInit {
    apostaForm: FormGroup;
    //nameCtrl: FormControl;
    //apostaCtrl: FormControl;
    gameType: number = 1;
    sorteado: string = '';
    errorMessage: string = '';
    public apostaList: Apostas[] = [];

    constructor(private formBuilder: FormBuilder, private _lotteryService: LotteryService) {

        this.apostaForm = this.formBuilder.group({
            gameType: 1
        });
    }

    ngOnInit() { }

    sortear() {
        this.errorMessage = '';

        this._lotteryService.getSorteio(this.apostaForm.value)
            .subscribe(data => {
                if (data.error === '') {
                    this.apostaList = data.vencedores;
                    this.sorteado = data.sorteio;
                }
                else {
                    this.errorMessage = data.error.split(',');
                }
            })
    }

    reset() {
        this._lotteryService.Reset().subscribe(data => { });

        this.apostaList = [];
        this.errorMessage = '';
        this.gameType = 1;
        this.sorteado = '';
    }
}

interface Apostas {
    Id: number;
    Name: string;
    Acertos: string;
    Erros: string;
}