import React, { Component } from 'react'
import Form from 'react-bootstrap/Form';

export default class RadioComp extends Component {
    render() {
        let group = this.props.group
        let radioButtons = this.props.radioButtons

        return (
            <div key={`radio`} className="mb-3">
                {radioButtons.map((button) => (

                    <Form.Check
                        defaultChecked={button.defaultChecked}
                        key={button.id}
                        type={'radio'}
                        id={button.id}
                        name={ group }
                        label={button.label}
                        onChange={() => button.callback(button.callbackValue)}
                    />
                ))}
            </div>
        )
    }
}
