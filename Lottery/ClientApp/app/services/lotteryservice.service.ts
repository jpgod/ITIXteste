import { Injectable, Inject } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Router } from '@angular/router';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

@Injectable()
export class LotteryService {
  myAppUrl: string = "";

  constructor(private _http: Http, @Inject('BASE_URL') baseUrl: string) {
    this.myAppUrl = baseUrl;
  }

  //Reset das apostas feitas
  Reset() {
      return this._http.get(this.myAppUrl + 'api/Lottery/Reset')
          .catch(this.errorHandler);
  }

  //Listar as apostas feitas
  getGamesList() {
    return this._http.get(this.myAppUrl + 'api/Lottery/GetGamesList')
      .map(res => res.json())
      .catch(this.errorHandler);
  }

  //Registrar uma aposta
  saveAposta(aposta) {
    return this._http.post(this.myAppUrl + 'api/Lottery/Apostar', aposta)
      .map((response: Response) => response.json())
      .catch(this.errorHandler)
  }

  //Realizar o sorteio e devolver a lista ganhadores (4, 5 ou 6 acertos)
  getSorteio(id) {
    return this._http.post(this.myAppUrl + 'api/Lottery/Sortear/', id)
      .map((response: Response) => response.json())
      .catch(this.errorHandler);
  }

  //Limpar as apostas existentes para jogar e sortear novamente
  resetApostas(id) {
    return this._http.delete(this.myAppUrl + "api/Lottery/Reset/" + id)
      .map((response: Response) => response.json())
      .catch(this.errorHandler);
  } 

  //tratamento de erro
  errorHandler(error: Response) {
      console.log(error);
      return Observable.throw(error);
  }
}  
