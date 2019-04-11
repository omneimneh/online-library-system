import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { SlideshowComponent } from './slideshow/slideshow.component';
import { BookThumbnailComponent } from './book-thumbnail/book-thumbnail.component';
import { HomeRootComponent } from './home-root/home-root.component';
import { HttpModule } from '@angular/http';
import { BookRootComponent } from './book-root/book-root.component';

@NgModule({
    imports: [BrowserModule, HttpModule],
    declarations: [SlideshowComponent, BookThumbnailComponent, HomeRootComponent, BookRootComponent],
    bootstrap: [HomeRootComponent]
})
class HomeModule { }

@NgModule({
    imports: [BrowserModule, HttpModule],
    declarations: [BookRootComponent],
    bootstrap: [BookRootComponent]
})
class BookModule { }

export { HomeModule, BookModule };