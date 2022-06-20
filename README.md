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

RapiTest is a web application for automated and semi-automated black-box testing of RESTful web APIs. It follows a model-based approach, where test cases are automatically derived from the OpenAPI Specification (OAS) of the API under test or manually derived from the Test Specification File (_TSL_). No access to the source code is required, which makes it possible to test APIs written in any programming language, running in local or remote servers.

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

If you want to run it directly without the use of docker here are the steps you need to make

#### Prerequisites

* Install Visual Studio 2019 (Compatibility with other versions is not guaranteed)
* Install Sql Server 2019 (Compatibility with other versions is not guaranteed)
* _Optional_ Install Sql Server Management Studio 
* Install RabbitMQ (Default Installation)
* Install Node.js and NPM

#### Installation

After having installed all the required software:

1. Clone the repo
   ```sh
   git clone https://github.com/DuarteFelicio/RAPITest.git
   ```
2. Create a Database
    ```sh
    Example name: RapiTestDB
    ```
3. Change Connection String Values<br/>
   Open the solution with Visual Studio and go to the _appsettings.json_ file of RAPITest, RunTestsWorkerService and SetupTestsWorkerService projects and change the line:
   ```sh
   //local
   "DefaultConnection": type your connection string here
   ```
4. Install NPM packages<br/>
   Open a command line in the folder _RAPITest\RAPITest\ClientApp_ and run the command
   ```sh
   npm install
   ```
5. Configure the solution<br/>
  Make sure the project is set to _run multiple projects_, _start_ RAPITest, RunTestsWorkerService and SetupTestsWorkerService and _none_ for ModelsLibrary
6. Create Database Tables<br/>
   Open the _package manager console_ (tools -> nuget manager -> package manager console)<br/>
   Make sure the default project is RAPITest<br/>
   Run the command:
   ```sh
   EntityFrameWorkCore\Update-Database -Context ApplicationDbContext
   ```
7. Run and enjoy!

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->
## Usage

Demonstration on youtube coming soon. 

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- ROADMAP -->
## Test Specific Language, _TSL_

<p>
As mentioned in the introduction, this app takes advantage of _TSL_ files. 
</p>
These are _YAML_ files with the purpose of defining specific tests based on HTTP requests in order to specify tests that couldn't reliably be made automaticly with just the API's specification. <br/>
The app supports the creation of these files trough a simple UI, however not every functionality is supported trough this UI, leaving some functionalities to only a manual creation. 

### Basic Structure 

You can write _TSL_ files in _YAML_. A sample _TSL_ definition written in _YAML_ looks like:
```sh
   - WorkflowID: crud_pet

    Stress:
      Count: 40
      Threads: 5
      Delay: 0

    Tests:

    - TestID: createPet
      Server: "https://petstore3.swagger.io/api/v3"
      Path: "/pet"
      Method: Post
      Headers:
        - Content-Type:application/json
        - Accept:application/json
      Body: "$ref/dictionary/petExample"
      Retain:
        - petId#$.id
      Verifications:
        - Code: 200
          Schema: "$ref/definitions/Pet"

    - TestID: readPet
      Server: "https://petstore3.swagger.io/api/v3"
      Path: "/pet/{petId}"
      Method: Get
      Headers:
        - Accept:application/xml
      Verifications:
        - Code: 200
          Schema: "$ref/definitions/Pet"
          Custom: ["CustomVerification.dll"]

    - TestID: updatePet
      Server: "https://petstore3.swagger.io/api/v3"
      Path: "/pet/{petId}"
      Query:
        - name=doggie
        - status=sold
      Method: Post
      Headers:
        - Accept:application/xml
      Verifications:
        - Code: 200
          Schema: "$ref/dictionary/petSchemaXml"

    - TestID: deletePet
      Server: "https://petstore3.swagger.io/api/v3"
      Path: "/pet/{petId}"
      Method: Delete
      Headers:
        - Accept:application/json
      Verifications:
        - Code: 200     
```
#### Workflow

Every _TSL_ file must include atleast one workflow 
```sh
  - WorkflowID: crud_pet
```
The workflow needs an ID, which must be unique across all workflows <br/>
One workflow must be comprised of one or more tests and optionally one stress test. All the tests inside the workflow are guaranteed to be executed sequentially which is usefull for situations where the output of one test influences the input of the other

#### Stress Test

Optionally, one workflow can have one stress test 
```sh
  Stress:
      Count: 40
      Threads: 5
      Delay: 0
```
The stress test has 3 fields, Count, Threads and Delay.<br/>
* Count defines the number of times the complete workflow will be executed
* Threads defines the number of threads by which _Count_ will be divided (Count: 10 and Threads: 2 means each thread will execute the workflow 5 times)
* Delay defines the delay in milliseconds between every full execution, which can be usefull to prevent errors of too many executions 

#### Test

You can define one or more tests within each workflow
```sh
  Tests:

    - TestID: createPet
      Server: "https://petstore3.swagger.io/api/v3"
      Path: "/pet"
      Method: Post
      Headers:
        - Content-Type:application/json
        - Accept:application/json
      Body: "$ref/dictionary/petExample"
      Retain:
        - petId#$.id
      Verifications:
        - Code: 200
          Schema: "$ref/definitions/Pet"

    - TestID: readPet
      Server: "https://petstore3.swagger.io/api/v3"
      Path: "/pet/{petId}"
      Method: Get
      Headers:
        - Accept:application/xml
      Verifications:
        - Code: 200
          Contains: id
          Count: doggie#1
          Schema: "$ref/definitions/Pet"
          Match: /Pet/name#doggie
          Custom: ["CustomVerification.dll"]

    - TestID: updatePet
      Server: "https://petstore3.swagger.io/api/v3"
      Path: "/pet/{petId}"
      Query:
        - name=doggie
        - status=sold
      Method: Post
      Headers:
        - Accept:application/xml
      Verifications:
        - Code: 200
          Schema: "$ref/dictionary/petSchemaXml"

    - TestID: deletePet
      Server: "https://petstore3.swagger.io/api/v3"
      Path: "/pet/{petId}"
      Method: Delete
      Headers:
        - Accept:application/json
      Verifications:
        - Code: 200     
```

The test is identified with its id, which MUST be unique across all tests of every workflow
```sh
   - TestID: createPet
```

It is then followed by 3 mandatory fields for a successful HTTP request, the server, the path and the method
```sh
    Server: "https://petstore3.swagger.io/api/v3"
    Path: "/pet"
    Method: Post
```
(Currently the only supported methods are Get, Post, Put and Delete)

You can also define headers following a key/value structure
```sh
  Headers:
    - Content-Type:application/json
    - Accept:application/json
```

Which is similar for Query string parameters
```sh
  Query:
    - name=doggie
    - status=sold
```

The body data can be defined directly on the _TSL_ file, however they can sometimes be extremely large which would hurt the clarity and readability of the file. You can however create an auxiliary text file (dictionary file) which contains the actual body and a unique identifier within the dictionary, leaving only the reference to said identifier on the _TSL_ file
```sh
  Body: "$ref/dictionary/petExample"
```

##### Dictionary File

The dictionary file is as mentioned before a text file containing all the body data and schemas in order to improve clarity on the actual _TSL_ file
```sh
  dictionaryID:petExample
  {
    "id": 10,
    "name": "doggie",
    "status": "available"
  }

```
Every entry on the file requires the dictionaryID which must be unique across all entries, followed by the actual data, followed by an empty line to separate them

##### Verifications

Each test can have multiple verifications, only the Code verification is mandatory
```sh
  Verifications:
    - Code: 200
      Contains: id
      Count: doggie#1
      Schema: "$ref/definitions/Pet"
      Match: /Pet/name#doggie
      Custom: ["CustomVerification.dll"]
```
Currently 6 different verifications are supported:

| Requirement 	| Name     	| Input Type        	| Description                                                     	|
|-------------	|----------	|-------------------	|-----------------------------------------------------------------	|
| Mandatory   	| Code     	| Integer           	| Response code matches the given code                            	|
| Optional    	| Contains 	| String            	| Response body contains the given string                         	|
| Optional    	| Count    	| String#Integer    	| Response body contains given string # times                     	|
| Optional    	| Schema   	| String            	| Response body matches the given schema*                          	|
| Optional    	| Match    	| StringPath#String 	| Response body matches the given value present in the StringPath 	|
| Optional    	| Custom   	| [String]          	| Runs Custom verifications given by the user                     	|

\*The schema verification can be supplied directly, or through reference to the dictionary file or to any schema present in the supplied OAS  

##### Retain

In some requests, the input is based on the output of a previous request, usually in simple workflows, like create read<br/>
```sh
    Retain:
      - petId#$.id
```
The keyword Retain allows the user to retain some information from the response body of the request to then be used in other tests of the same workflow<br/>
For instance the value present at the json path _$.id_ will be retained with the identifier _petId_ which can then be used in following tests
```sh
    Path: "/pet/{petId}"
```

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
