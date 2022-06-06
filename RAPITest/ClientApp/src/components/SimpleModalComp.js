import React from 'react'
import { Modal } from 'react-bootstrap'
import { AwesomeButton } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";

export default class SimpleModalComp extends React.Component {
    render() {
        let title = this.props.title
        let body = this.props.body
        let cancelButtonFunc = this.props.cancelButtonFunc
        let visible = this.props.visible

        return (
            <div>
                <Modal size="lg" show={visible} onHide={cancelButtonFunc}>
                    <Modal.Header closeButton>
                        <Modal.Title>{title}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <pre>{body}</pre>
                    </Modal.Body>
                    <Modal.Footer>
                        <AwesomeButton type="secondary" onPress={cancelButtonFunc}>Close</AwesomeButton>
                    </Modal.Footer>
                </Modal>
            </div>
        )
    }
}