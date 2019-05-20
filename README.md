# Online Library System

## Quick Notes

A simple and easy to use library system for universities, that allows students and professors to request a book online and provide admins with an interface to checkout orders and maintain the books data.

<br>

## Guide

### Online Demo

* Just go to [http://www.onlinelibrarysystem.somee.com/](http://www.onlinelibrarysystem.somee.com/) for a live demo!

### Compile and Run the Source Code

#### Requirements
  * Operating System: Windows 10
  * Microsoft SQL Server 2017
  * Microsoft Visual Studio 2015 or later version
  * Visual Studio package `Microsoft.VisualStudio.Workload.NetWeb` must be installed
  
#### Steps
  * Clone or download the source code from [here](https://github.com/omneimneh/online-library-system/archive/master.zip)
  * Open the solution in Visual Studio
  * Open Microsoft Sql Server 2017
  * Copy contents in the files `/SqlScripts/DDL.sql` and `/SqlScripts/DML.sql` respectively to a new query window in Sql Server
  * Run the query and refresh the databases in the Object Explorer, you should find `OnlineLibrarySystem` in your databases list
  * Open `/OnlineLibrarySystem/Web.config` and edit your connection string, replace the server name with your PC name.
  * Go back to visual studio, in the solution explorer right click on the file `/OnlineLibrarySystem/packages.json` and click on `Restore packages`, this will trigger the command `npm install` to install the required `node_modules` since we're using angular
  * Press F5 to build and run the project

<br>

## Project Structure

### ASP.NET MVC 4

* **Model:** Classes representing the objects and entities are found in `/OnlineLibrarySystem/Model`
* **View:** Razor Pages (`*.cshtml`) are found in `/OnlineLibrarySystem/Views`
  * `Shared/_Layout` is the master page
  * The shared layout define several sections, scripts are loaded at the end of the document (for faster load time)
* **Controller:** The classes that feed the Views with the Model data are found in `/OnlineLibrarySystem/Controller`
  * `ApiControllers` begin with the prefix `Api` define some actions that can modify or get results from the database, some actions are only accessible by a certain type of user
  * `Controllers` Actions that return a View whenever it can be accessed, the `BaseController` define the permissions depending on the user role.
  
### Angular 5

The routing is handeled by ASP.NET MVC, so angular is used without routing.
Some but not all pages are made with angular. Angular components and layouts are found in the `/OnlineLibrarySystem/app` folder

### JavaScript & CSS Files

  * Scripts are found in `/OnlineLibrarySystem/Scripts`
  * Stylesheets are found in `/OnlineLibrarySystem/Content/css`

### Other Libraries
  * jQuery
  * Bootstrap
  * Date.js
  
### Database Structure

#### Entity Relations Diagram

![ER Diagram](https://github.com/omneimneh/online-library-system/blob/master/SqlScripts/diagrams/ER_Diagram.png)

<br>

## Additional Notes

#### Queries used are saved under

  * `/SqlScripts/DDL.sql` Data definition, creates the database, tables and views
  * `/SqlScripts/DML.sql` Data manipulation, inserts a dummy data and setup the admin account
  * `/SqlScripts/ApiQueries.sql` Queries that were used inside the Api, paramaters are not supplied!

#### Additional files

  * `/SqlScripts/diagrams/ER_Diagram.PNG` Entity relations diagram
  * `/SqlScripts/database/OnlineLibrarySystem.mdf` generated database
