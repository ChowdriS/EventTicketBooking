import { Component, signal } from '@angular/core';
import { EventService } from '../../services/Event/event.service';
import { Router } from '@angular/router';
import { Getrole } from '../../misc/Token';
import { Auth } from '../../services/Auth/auth';

@Component({
  selector: 'app-slider',
  imports: [],
  templateUrl: './slider.html',
  styleUrl: './slider.css',
  standalone : true
})
export class Slider {
  images = signal<any | null>(null);
  currentIndex = signal<number>(0);
  intervalId: any;
  role : string = '';
  constructor(private eventsService: EventService,private auth : Auth,public router : Router) {}
  
  ngOnInit() {
    this.GetAllEventImages();
    this.startSlider();
    this.role = Getrole(this.auth.getToken());
  }
  routeToEvent(img:any){
    let eventId = img.eventId;
    this.router.navigate([this.router.url,'events', eventId]);
  }
  GetAllEventImages() {
    if(this.role != 'Manager'){
      this.eventsService.getAllEventImages().subscribe({
        next:(res:any)=>{
          this.images.set(res.$values);
          // console.log(this.images());
        },
        error:(err:any)=>{
          console.log(err.message)
        }
      })
    }
    else{
      this.eventsService.getMyEventImages().subscribe({
        next:(res:any)=>{
          this.images.set(res.$values);
          // console.log(this.images());
        },
        error:(err:any)=>{
          console.log(err.message)
        }
      })
    }
  }
  startSlider() {
    this.intervalId = setInterval(() => {
      const count = this.images()?.length || 0;
      if (count > 0) {
        const next = ((this.currentIndex()) + 1) % count;
        this.currentIndex.set(next);
      }
    }, 3500); 
  }

  goToSlide(index: number) {
    this.currentIndex.set(index);
  }
  getCurrentIndex(){
    return this.currentIndex();
  }
}