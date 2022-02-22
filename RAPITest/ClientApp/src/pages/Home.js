import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import CardComp from '../components/CardComp'
import titleBackground from '../assets/titleBackground.jpg'
import statsIcon from '../assets/stats.png'
import uploadIcon from '../assets/upload.png'
import loginIcon from '../assets/login.png'
import { Container, Row, Col, Figure} from 'react-bootstrap'
import './Home.css';
import { Login } from './api-authorization/Login';
import { AwesomeButton } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";

export class Home extends Component {

    static displayName = Home.name;

    constructor(props) {
        super(props)
        this.state = {
            Auth: false,
            userName: "",
            currentUploadedFiles: 0,
            lastActions: [],
            currentAnalysedFiles: 0,
            localUploaded: 0,
            urlUploaded: 0,
            lastLogin: "",
            activeIndex: 0,
            render: false
        }
    }

    //check if user is autenticated, if not render notAuth vs render Auth
    async componentDidMount() {
        let token = await authService.getAccessToken()
        if (token !== null) {
            //fetch para saber os dados do user
            fetch(`Home/GetUserDetails`, {
                method: 'GET',
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
            }).then(res => {
                if (res.status !== 204) {
                    res.json().then(details => {
                        let lastLoginString = details.lastLogin.split('T')
                        let finalString = lastLoginString[0] + ' ' + lastLoginString[1].split('.')[0]
                        this.setState({
                            Auth: true,
                            userName: details.userName,
                            currentUploadedFiles: details.currentUploadedFiles,
                            lastActions: details.lastActions,
                            currentAnalysedFiles: details.currentAnalysedFiles,
                            localUploaded: details.localUploaded,
                            urlUploaded: details.urlUploaded,
                            lastLogin: finalString,
                            activeIndexAnalysed: 0,
                            activeIndexUploaded: 0,
                            render: true
                        })
                    })
                }
                else {
                    this.setState({ render: true })
                }
            })
        }
        else {
            this.setState({
                render: true
            })
        }

    }

    onPieEnterUploaded = (data, index) => {
        this.setState({
            activeIndexUploaded: index,
        });
    };

    onPieEnterAnalysed = (data, index) => {
        this.setState({
            activeIndexAnalysed: index,
        });
    };

    renderLastActions(lastActions) {
        return lastActions.map(action => {
            if (action.Action === 'Analyze') {
                return <div class="row" style={{ paddingLeft: 10 }}><a href={'/workspace/analysis/' + action.CsvFileId}>The analysis for the file {action.FileName} is finished!</a></div>
            }
            return <div class="row" style={{ paddingLeft: 10 }}><a href={'/workspace/analysis/' + action.CsvFileId + '?Version=' + action.Version}>You were working on {action.FileName} (version {action.Version})</a></div>
        })
    }

    //render page for a logged in user
    renderAuth() {
        const analysedFilesData = [
            { name: 'Not Analysed', value: this.state.currentUploadedFiles - this.state.currentAnalysedFiles },
            { name: 'Analysed', value: this.state.currentAnalysedFiles }
        ];

        const uploadedURL = [
            { name: 'Files Uploaded Locally', value: this.state.localUploaded },
            { name: 'Files Uploaded URL', value: this.state.urlUploaded }
        ];

        return (
            <div>
                <div style={{ backgroundImage: "url(" + titleBackground + ")", padding: "10px 0px 10px 0px" }}>
                    <h1 class="row justify-content-md-center" style={{ width: "100%" }}>Welcome back {this.state.userName}!</h1>
                    <h4 class="row justify-content-md-center" style={{ marginTop: 25, width: "100%" }}>Here is some general data about you:</h4>
                </div>
                <Container style={{ fontFamily: 'Open Sans' }}>
                    <div class="row" style={{ marginTop: 30 }}>
                        <div class="col-6" style={{ marginLeft: 161 }}>
                            <CardComp
                                title='Previous Actions'
                                body={this.renderLastActions(this.state.lastActions)}
                            />
                        </div>
                        <div class="col-3">
                            <div class="row">
                                <CardComp
                                    title='Uploaded Files'
                                    body={this.state.currentUploadedFiles}
                                />
                            </div>
                            <div class="row" style={{ marginTop: "15px" }}>
                                <CardComp
                                    title='Previous Login'
                                    body={this.state.lastLogin}
                                />
                            </div>
                        </div>
                    </div>

                </Container>
                <div class="row" style={{ maxWidth: 1919 }}>
                    <div class="col-6">
                        
                    </div>
                    <div>
                        
                    </div>
                </div>
            </div>
        )
    }

    //render page for a not logged in user
    renderNotAuth() {
        return (
            <div>
                <Row style={{ textAlign: "center" }}>
                    <h1>RAPITest Helps You</h1>
                    <h1 style={{ fontWeight: "bold" }}>Validate API's Continuously</h1>
                    <h3>With 3 simple steps</h3>
                </Row>
                <Row style={{ textAlign: "center", paddingTop: "100px" }}>
                    <Col>
                        <Figure>
                            <Figure.Image
                                width={171}
                                height={180}
                                alt="171x180"
                                src={loginIcon}
                            />
                            <Figure.Caption>
                                Register or Login
                            </Figure.Caption>
                        </Figure>
                    </Col>
                    <Col style={{ paddingTop: "100px" }}>
                        <Figure>
                            <Figure.Image
                                width={171}
                                height={180}
                                alt="171x180"
                                src={uploadIcon}
                            />
                            <Figure.Caption>
                                Setup your test configuration
                            </Figure.Caption>
                        </Figure>
                    </Col>
                    <Col>
                        <Figure>
                            <Figure.Image
                                width={171}
                                height={180}
                                alt="171x180"
                                src={statsIcon}
                            />
                            <Figure.Caption>
                                Analyse the results
                            </Figure.Caption>
                        </Figure>
                    </Col>
                </Row>
                <Row style={{ padding: "50px 550px 0px 550px" }}>
                    <AwesomeButton type="primary" onPress={() => this.props.history.push('setupTest') }>Get Started!</AwesomeButton>
                </Row >
            </div>
        );
    }

    render() {
        if (this.state.render) {
            return (
                <div>
                    {this.state.Auth ? this.renderAuth() : this.renderNotAuth()}
                </div>
            )
        }
        return <div></div>
    }
}

