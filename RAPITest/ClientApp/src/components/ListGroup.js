﻿import React, { Component } from 'react'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap';

export default class ListGroup extends Component {

    render() {
        let title = this.props.title
        let files = this.props.files
        let symbol = this.props.symbol
        let toShow = this.props.toShow

        return (
            <div>
                <h4>{title}</h4>
                <ul className="list-group">
                    {
                        files.map((f,i) => <li key={i} className="list-group-item">
                            <div className="row">
                                <div className="column">
                                    {symbol}
                                </div>
                                {toShow(f)}
                            </div>
                        </li>)
                    }
                </ul>
            </div>
        )
    }
}