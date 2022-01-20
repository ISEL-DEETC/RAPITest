import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';


export class TimeLoop extends Component {

    constructor() {

        super()

        this.state = {
            step: 4
        }
    }

    render() {
        return  <button type="button" className="btn btn-outline-primary" onClick={() => this.props.handler(this.state.step)}>Finalize</button>
    }
}