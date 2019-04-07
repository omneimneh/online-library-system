
class Category {
    name: string;
    constructor(name: string) {
        this.name = name;
    }
}

class Book {
    title: string;
    author: string;
    categories: Category[];


    constructor(title: string, author: string, categories: Category[]) {
        this.author = author;
        this.title = title;
        this.categories = categories;
    }
}

export { Category, Book };