import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import Form from 'react-bootstrap/Form';
import RadioComp from '../../components/RadioComp'
import { warningMessage } from '../../components/AlertComp'

export class TimeLoop extends Component {

    constructor() {

        super()

        this.state = {
            runimmediately: true,
            selectedRadioLabel: "1 hour",
            radioButtons: [],
            group: "group1",
            showWarning: false
        }

        this.finalizeCallback = this.finalizeCallback.bind(this)
        this.changeSelectedRadio = this.changeSelectedRadio.bind(this)
        this.handleCheck = this.handleCheck.bind(this)
        this.closeWarning = this.closeWarning.bind(this)
    }

    componentDidMount() {
        let button1H = {
            defaultChecked: true,
            id: "radio1H",
            label: "1 hour",
            callback: this.changeSelectedRadio,
            callbackValue: "1 hour"
        }
        let button12H = {
            defaultChecked: false,
            id: "radio12H",
            label: "12 hours",
            callback: this.changeSelectedRadio,
            callbackValue: "12 hours"
        }
        let button24H = {
            defaultChecked: false,
            id: "radio24H",
            label: "24 hours",
            callback: this.changeSelectedRadio,
            callbackValue: "24 hours"
        }
        let button1W = {
            defaultChecked: false,
            id: "radio1W",
            label: "1 week",
            callback: this.changeSelectedRadio,
            callbackValue: "1 week"
        }
        let button1M = {
            defaultChecked: false,
            id: "radio1M",
            label: "1 month",
            callback: this.changeSelectedRadio,
            callbackValue: "1 month"
        }
        let buttonNever = {
            defaultChecked: false,
            id: "radioNever",
            label: "Never",
            callback: this.changeSelectedRadio,
            callbackValue: "Never"
        }
        this.setState({ radioButtons: [button1H, button12H, button24H, button1W, button1M, buttonNever] })
    }

    finalizeCallback() {
        if (this.state.selectedRadioLabel === "Never" && !this.state.runimmediately) {
            this.setState({ showWarning: true })
            return
        }
        let ret = {
            runimmediately: this.state.runimmediately,
            interval: this.state.selectedRadioLabel
        }
        this.props.handlerTime(ret)
    }

    changeSelectedRadio(newRadio) {
        this.setState({ selectedRadioLabel: newRadio })
    }

    handleCheck() {
        let newrun = !this.state.runimmediately
        this.setState({ runimmediately: newrun })
    }

    closeWarning() {
        this.setState({ showWarning: false })
    }

    render() {
        return (
            <div>
                {this.state.showWarning ? warningMessage("Please select either run immediately or one of the intervals", this.closeWarning) : <div></div>}
                <Form>
                    <div key={`checkbox`} className="mb-3">
                        <Form.Check
                            defaultChecked
                            type={'checkbox'}
                            id={`testImmediately`}
                            label={`Run tests immediately after setup is finished?`}
                            onChange={this.handleCheck}
                        />
                    </div>
                    <h4>Run tests every</h4>
                    <RadioComp
                        group={this.state.group}
                        radioButtons={this.state.radioButtons}
                    />
                </Form>
                <button type="button" className="btn btn-outline-primary" onClick={this.finalizeCallback}>Finalize</button>
            </div>
        )
    }
}