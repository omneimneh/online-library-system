import { Category, Book } from "./Model";

let someCategories = [
    new Category('math'), new Category('computer-science'), new Category('medicine')
];

let someBooks = [
    new Book('MVC web apps', 'John Smith', someCategories),
    new Book('Get over it', 'virus.exe', someCategories),
    new Book('Angular', 'Google', someCategories),
    new Book('React js', 'Facebook', someCategories),
    new Book('AWS', 'Amazon', someCategories),
    new Book('Intro to physics', 'Newton', someCategories)
];

let appTitle: string = "Online Library System";

export { someBooks, someCategories, appTitle };