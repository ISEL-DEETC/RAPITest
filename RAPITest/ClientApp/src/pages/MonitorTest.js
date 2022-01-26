import React, { Component } from 'react';
import { Container } from 'reactstrap';
import authService from './api-authorization/AuthorizeService'
import 'bootstrap/dist/css/bootstrap.css'
import { Form, Col, InputGroup, FormControl } from 'react-bootstrap'
import 'bootstrap';
import './MonitorTest.css'
import ModalComp from '../components/ModalComp.js'
import Loader from 'react-loader-spinner'

export class MonitorTest extends Component {

    static displayName = MonitorTest.name;

    constructor(props) {
        super(props)
        this.state = {
            apis: new Map(),
            requestLoops: [],
            onShowDeleteModal: false,
            searchByName: '',
            idToRemove: -1
        }
        this.enableDeleteModal = this.enableDeleteModal.bind(this)
        this.disableDeleteModal = this.disableDeleteModal.bind(this)
        this.removeFile = this.removeFile.bind(this)
        this.Remove = this.Remove.bind(this)
        this.handleOnChange = this.handleOnChange.bind(this)
    }

    handleOnChange(e) {
        this.setState({
            [e.target.name]: e.target.value
        })
    }

    //get user apis
    async componentDidMount() {
        const token = await authService.getAccessToken();
        const response = await fetch('MonitorTest/GetUserAPIs', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        })
        const data = await response.json()
        console.log(data)
        var allAPIS = new Map()
        data.forEach(api => {
            allAPIS.set(api.APITitle, api)
        })
        this.setState({ apis: allAPIS })
        //data.forEach(f => {
        //    if (f.isAnalysing === true) {
        //        this.checkAnalysisStatus(f.apiTitle, token)
        //    }
        //})
    }

    async componentWillUnmount() {
        this.state.requestLoops.forEach(rl => {
            clearInterval(rl)
        })
    }

    removeFile(Id) {
        var mapAux = this.state.apis
        mapAux.delete(Id)
        this.setState({
            apis: mapAux
        })
    }

    //callback for Analyse button
    async Analyze(id) {
        const token = await authService.getAccessToken();
        var map = this.state.apis
        fetch(`Workspace/AnalyseFile?fileId=${id}`, {
            method: 'GET',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        }).then(res => res.json())
            .then(newFile => {
                map.set(id, newFile)
                this.setState({ apis: map })
            })
        this.checkAnalysisStatus(id, token)
    }

    //Interval to check with backend if the analysis is completed
    async checkAnalysisStatus(id, token) {
        var map = this.state.apis
        var requestLoop = setInterval(function () {
            fetch(`Workspace/IsAnalysisComplete?fileId=${id}`, {
                method: 'GET',
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
            }).then(res => {
                if (res.status !== 204) { //204 = empty response -> not yet completed analysis
                    res.json().then(newFile => {
                        map.set(id, newFile)
                        stopLoop(map)
                    })
                }
            })
        }, 5000); //5 seconds
        this.setState({ requestLoops: this.state.requestLoops.concat(requestLoop) })

        var stopLoop = (newMap) => {
            this.setState({ apis: newMap })
            this.forceUpdate()
            clearInterval(requestLoop)
        }
    }

    //redirect to Analysis
    async Analyzis(id) {
        this.props.history.push(`/workspace/analysis/${id}`)
    }

    //callback for download analysis button
    async DownloadAnalyzis(id, fileName) {
        const token = await authService.getAccessToken();

        fetch(`Workspace/DownloadAnalysis?fileId=${id}`, {
            method: 'GET',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        }).then(response => {
            response.blob().then(blob => {
                let url = window.URL.createObjectURL(blob);
                let a = document.createElement('a');
                a.href = url;
                a.download = fileName.split('.')[0] + '_analysis' + '.json';
                a.click();
            });
        });

    }

    enableDeleteModal(id) { this.setState({ onShowDeleteModal: true, idToRemove: id }) }

    disableDeleteModal() { this.setState({ onShowDeleteModal: false }) }

    //delete a file from workspacce
    async Remove() {
        const token = await authService.getAccessToken();
        let id = this.state.idToRemove
        fetch(`Workspace/RemoveFile?fileId=${id}`, {
            method: 'DELETE',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        }).then(res => {
            this.removeFile(id)
            this.disableDeleteModal()
        })
    }

    render() {

        return (
            <div style={{ backgroundColor: "#F0F0F0", minHeight: "808px" }}>
                <div>
                    <h1 className="row justify-content-md-center" style={{ width: "100%" }}>Monitor Tests</h1>
                    <h4 className="row justify-content-md-center" style={{ marginTop: 25, width: "100%" }}>Search, Analyse and Visualize.</h4>
                </div>
                <Container style={{ marginTop: "20px", fontFamily: 'Open Sans' }}>
                    <div className="row">
                        <Col sm={6}>
                            <InputGroup className="mb-2">
                                <InputGroup.Prepend>
                                    <InputGroup.Text id="basic search">{
                                        <svg width="1em" height="1em" viewBox="0 0 16 16" className="bi bi-search" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
                                            <path fillRule="evenodd" d="M10.442 10.442a1 1 0 0 1 1.415 0l3.85 3.85a1 1 0 0 1-1.414 1.415l-3.85-3.85a1 1 0 0 1 0-1.415z" />
                                            <path fillRule="evenodd" d="M6.5 12a5.5 5.5 0 1 0 0-11 5.5 5.5 0 0 0 0 11zM13 6.5a6.5 6.5 0 1 1-13 0 6.5 6.5 0 0 1 13 0z" />
                                        </svg>
                                    }</InputGroup.Text>
                                </InputGroup.Prepend>
                                <FormControl placeholder="Search by name ..." name="searchByName" type="text" onChange={this.handleOnChange} />
                            </InputGroup>
                        </Col>
                    </div>
                    <div className="row">
                        <div className="col-sm-6">
                            <div className="list-group" id="list-tab" role="tablist">
                                {Array.from(this.state.apis).map(([key, item]) => {
                                    if (item.apiTitle.toLowerCase().includes(this.state.searchByName.toLowerCase()))
                                        return <a className="list-group-item list-group-item-action" id={'list-' + item.apiTitle} data-toggle="list" href={'#details-' + item.apiTitle} role="tab" >
                                            <div className="row" style={{ height: 30 }}>
                                                <div className="column">
                                                    <svg className="bi bi-file-text" width="100" height="35" viewBox="0 0 16 16" fill="currentColor" xmlns="http://www.w3.org/2000/svg" ><path fill-rule="evenodd" d="M4 1h8a2 2 0 012 2v10a2 2 0 01-2 2H4a2 2 0 01-2-2V3a2 2 0 012-2zm0 1a1 1 0 00-1 1v10a1 1 0 001 1h8a1 1 0 001-1V3a1 1 0 00-1-1H4z" clip-rule="evenodd" /><path fill-rule="evenodd" d="M4.5 10.5A.5.5 0 015 10h3a.5.5 0 010 1H5a.5.5 0 01-.5-.5zm0-2A.5.5 0 015 8h6a.5.5 0 010 1H5a.5.5 0 01-.5-.5zm0-2A.5.5 0 015 6h6a.5.5 0 010 1H5a.5.5 0 01-.5-.5zm0-2A.5.5 0 015 4h6a.5.5 0 010 1H5a.5.5 0 01-.5-.5z" clip-rule="evenodd" /></svg>
                                                </div>
                                                <div className="column">
                                                    {item.apiTitle.length > 55 ? item.apiTitle.substring(0, 52) + '...' : item.apiTitle}
                                                </div>
                                            </div>
                                        </a>
                                })}
                            </div>
                        </div>
                        <div className="col-6">
                            <div className="tab-content" id="nav-tabContent" >
                                {Array.from(this.state.apis).map(([key, item]) => {
                                    return <div style={{ borderColor: "#45ABD1", borderStyle: "solid", borderRadius: "20px", padding: 7 }} class="tab-pane fade" id={'details-' + item.apiTitle} role="tabpanel" aria-labelledby={'list-' + item.apiTitle}>

                                        <table className="table table-striped">
                                            <tbody>
                                                <tr><th>Errors</th><td>{item.Errors}</td></tr>
                                                <tr><th>Warnings</th><td>{item.Warnings}</td></tr>
                                                <tr><th>Latest Report</th><td>{item.LatestReport}</td></tr>
                                                <tr><th>Next Text</th><td>{item.NextTest}</td></tr>
                                                {item.LatestReport !== null && this.renderAPITestInfo(item)}
                                            </tbody>
                                        </table>
                                        <div className="row" style={{ paddingLeft: "24px" }}>
                                            {item.LatestReport !== null && this.renderAnalysisButton(item)}
                                        </div>
                                    </div>
                                })}
                            </div>
                        </div>
                    </div>
                    <ModalComp
                        title="Delete File"
                        body="Are you sure you want to delete this file. This will delete every analysis (if any) as well as the file itself."
                        okButtonText="Delete"
                        okButtonFunc={this.Remove}
                        cancelButtonFunc={this.disableDeleteModal}
                        visible={this.state.onShowDeleteModal}
                    />
                </Container>
            </div>
        )
    }
}