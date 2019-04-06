import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { SlideshowComponent } from './slideshow/slideshow.component';

@NgModule({
    imports: [BrowserModule],
    declarations: [SlideshowComponent],
    bootstrap: [SlideshowComponent]
})
export class AppModule { }
