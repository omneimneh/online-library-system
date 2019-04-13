import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { SlideshowComponent } from './slideshow/slideshow.component';
import { BookThumbnailComponent } from './book-thumbnail/book-thumbnail.component';
import { HomeRootComponent } from './home-root/home-root.component';
import { HttpModule } from '@angular/http';
import { BookRootComponent } from './book-root/book-root.component';
import { SearchRootComponent } from './search-root/search-root.component';
import { BookResultComponent } from './book-result/book-result.component';

@NgModule({
    imports: [BrowserModule, HttpModule],
    declarations: [SlideshowComponent, BookThumbnailComponent, HomeRootComponent, BookRootComponent, SearchRootComponent, BookResultComponent],
    bootstrap: [HomeRootComponent]
})
class HomeModule { }

@NgModule({
    imports: [BrowserModule, HttpModule],
    declarations: [BookRootComponent],
    bootstrap: [BookRootComponent]
})
class BookModule { }

@NgModule({
    imports: [BrowserModule, HttpModule],
    declarations: [SearchRootComponent, BookResultComponent],
    bootstrap: [SearchRootComponent]
})
class SearchModule { }

export { HomeModule, BookModule, SearchModule };