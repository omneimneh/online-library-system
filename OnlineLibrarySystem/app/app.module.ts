import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { SlideshowComponent } from './slideshow/slideshow.component';
import { BookThumbnailComponent } from './book-thumbnail/book-thumbnail.component';
import { HomeRootComponent } from './home-root/home-root.component';

@NgModule({
    imports: [BrowserModule],
    declarations: [SlideshowComponent, BookThumbnailComponent, HomeRootComponent],
    bootstrap: [HomeRootComponent]
})
class HomeModule { }

export { HomeModule };