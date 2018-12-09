import { NgModule } from '@angular/core';
import { LotteryService } from './services/lotteryservice.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';  
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { ApostaComponent } from './components/aposta/aposta.component';
import { SorteioComponent } from './components/sorteio/sorteio.component';
import { NgxMaskModule } from 'ngx-mask';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        ApostaComponent,
        SorteioComponent,
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'apostar', component: ApostaComponent },
            { path: 'sortear', component: SorteioComponent },
            { path: '**', redirectTo: 'home' }
        ]),
        //NgxMaskModule,

    ],
    providers: [LotteryService]
})
export class AppModuleShared {
}  