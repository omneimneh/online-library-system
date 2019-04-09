import { Book } from "./Model";

let someBooks = [
    new Book('MVC web apps', 'John Smith'),
    new Book('Get over it', 'virus.exe'),
    new Book('Angular', 'Google'),
    new Book('React js', 'Facebook'),
    new Book('AWS', 'Amazon'),
    new Book('Intro to physics', 'Newton')
];

let appTitle: string = "Online Library System";

export { someBooks, appTitle };