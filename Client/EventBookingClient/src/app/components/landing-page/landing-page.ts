import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/Auth/auth';
import { Getrole } from '../../misc/Token';
import { NgOptimizedImage } from '@angular/common';
import { Slider } from "../slider/slider";

@Component({
  selector: 'app-landing-page',
  imports: [RouterLink],
  templateUrl: './landing-page.html',
  styleUrl: './landing-page.css',
  standalone: true
})
export class LandingPage {
  constructor(private authService : Auth,private router : Router){}
      ngOnInit() {
        if (this.authService.getToken()) {
          let token = this.authService.getToken();
          // console.log(token);
          let role = Getrole(token);
          if(role === 'User')
            this.router.navigate(['/user']);
          else if(role === 'Manager')
            this.router.navigate(['/manager']);
          else if(role === 'Admin')
            this.router.navigate(['/admin']);
        }
    }
}