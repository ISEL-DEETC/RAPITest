import React, { Component } from 'react';
import { Container } from 'reactstrap';
import authService from './api-authorization/AuthorizeService'
import 'bootstrap/dist/css/bootstrap.css'
import { Form, Col, InputGroup, FormControl } from 'react-bootstrap'
import 'bootstrap'
import './MonitorTest.css'
import ModalComp from '../components/ModalComp.js'
import Loader from 'react-loader-spinner'

export class VisualizeReport extends Component {

    static displayName = VisualizeReport.name;

    constructor(props) {
        super(props)
        this.state = {
            report: null
        }
        
    }

    async componentDidMount() {
        const token = await authService.getAccessToken();
        let apiTitle = this.props.match.params.apiTitle
        
        fetch(`MonitorTest/ReturnReport?apiTitle=${apiTitle}`, {
            method: 'GET',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        }).then(res => res.json())
            .then(report => {
                console.log(report)
                this.setState({report: report})
            })

    }

    render() {
        return (
            <div>todo</div>
        )
    }
}