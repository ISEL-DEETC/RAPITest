<div id="top"></div>
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Don't forget to give the project a star!
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->



<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/DuarteFelicio/RAPITest">
    <img src="images/logo.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">RapiTest</h3>

  <p align="center">
    Validate API's Continuously with RapiTest!
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

<img src="images/screenshot.png" alt="Logo" width="200" height="200">

RapiTest is a web application for automated and semi-automated black-box testing of RESTful web APIs. It follows a model-based approach, where test cases are automatically derived from the OpenAPI Specification (OAS) of the API under test or manually derived from the Test Specification File (TSL). No access to the source code is required, which makes it possible to test APIs written in any programming language, running in local or remote servers.

The test cases derived from the TSL file allow for some greater customization compared to other tools that only use the OAS, such as:
* Custom HTTP query strings or headers for specific APIs that need an API key
* Native or Custom verifications to specific HTTP requests
* Workflow testing of certain endpoints
* Stress tests to specific workflows

<p align="right">(<a href="#top">back to top</a>)</p>



### Built With

This app was developed using these frameworks: 

* [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-6.0)
* [React.js](https://reactjs.org/)
* [Bootstrap](https://getbootstrap.com)
* [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-2019)
* [RabbitMQ](https://www.rabbitmq.com/)

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

### Docker

The latest versions of the app are available as images in DockerHub, the docker-compose file is available under the folder _dockercompose_, simply copy it and run the command:

* docker-compose
  ```sh
  docker-compose up
  ```

### Locally

If you want to run it directly without the use of docker here are the steps you need to make.

#### Prerequisites

* Install Visual Studio 2019 (Compatibility with other versions is not guaranteed)
* Install Sql Server 2019 (Compatibility with other versions is not guaranteed)
* _Optional_ Install Sql Server Management Studio 
* Install RabbitMQ (Default Installation)
* Install Node.js and NPM

#### Installation

After having installed are the required software:

1. Clone the repo
   ```sh
   git clone https://github.com/DuarteFelicio/RAPITest.git
   ```
2. Create a Database
    ```sh
    Example name: RapiTestDB
    ```
3. Change Connection String values
   Open the solution with Visual Studio and go to the appsettings.json file of RAPITest, RunTestsWorkerService and SetupTestsWorkerService projects and change the line:
   ```sh
   //local
   \"DefaultConnection\": _type your connection string here_
   ```
4. Create Database Tables
   Open the _package manager console_ (tools -> nuget manager -> package manager console)
   Make sure the default project is RAPITest
   Run the command:
   ```sh
   EntityFrameWorkCore\\Update-Database -Context ApplicationDbContext
   ```
5. Install NPM packages
   Open a command line in the folder _RAPITest\RAPITest\ClientApp_ and run the command
   ```sh
   npm install
   ```
6. Make sure the project is set to _run multiple projects_, _start_ RAPITest, RunTestsWorkerService and SetupTestsWorkerService and _none_ for ModelsLibrary
7. Run and enjoy!

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->
## Usage

Demonstration on youtube coming soon. 

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- ROADMAP -->
## Test Specific Language, TSL

- [x] Add Changelog
- [x] Add back to top links
- [ ] Add Additional Templates w/ Examples
- [ ] Add "components" document to easily copy & paste sections of the readme
- [ ] Multi-language Support
    - [ ] Chinese
    - [ ] Spanish

See the [open issues](https://github.com/othneildrew/Best-README-Template/issues) for a full list of proposed features (and known issues).

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

Duarte Felício  - A42197@alunos.isel.pt

Project Link: [https://github.com/DuarteFelicio/RAPITest](https://github.com/DuarteFelicio/RAPITest)

<p align="right">(<a href="#top">back to top</a>)</p>





<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/othneildrew/Best-README-Template.svg?style=for-the-badge
[contributors-url]: https://github.com/othneildrew/Best-README-Template/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/othneildrew/Best-README-Template.svg?style=for-the-badge
[forks-url]: https://github.com/othneildrew/Best-README-Template/network/members
[stars-shield]: https://img.shields.io/github/stars/othneildrew/Best-README-Template.svg?style=for-the-badge
[stars-url]: https://github.com/othneildrew/Best-README-Template/stargazers
[issues-shield]: https://img.shields.io/github/issues/othneildrew/Best-README-Template.svg?style=for-the-badge
[issues-url]: https://github.com/othneildrew/Best-README-Template/issues
[license-shield]: https://img.shields.io/github/license/othneildrew/Best-README-Template.svg?style=for-the-badge
[license-url]: https://github.com/othneildrew/Best-README-Template/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/othneildrew
[product-screenshot]: images/screenshot.png
