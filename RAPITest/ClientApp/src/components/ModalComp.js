import React from 'react'
import { Modal} from 'react-bootstrap'
import { AwesomeButton, AwesomeButtonProgress } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";

export default class ModalComp extends React.Component {
    render() {
        let title = this.props.title
        let body = this.props.body
        let okButtonText = this.props.okButtonText
        let okButtonFunc = this.props.okButtonFunc
        let cancelButtonFunc = this.props.cancelButtonFunc
        let visible = this.props.visible

        return (
            <div>
                <Modal show={visible} onHide={cancelButtonFunc}>
                    <Modal.Header closeButton>
                        <Modal.Title>{title}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        {body}
                    </Modal.Body>
                    <Modal.Footer>
                        <AwesomeButtonProgress type="primary" onPress={(element, next) => { okButtonFunc(next)}}>{okButtonText}</AwesomeButtonProgress>
                        <AwesomeButton type="secondary" onPress={cancelButtonFunc}>Close</AwesomeButton>
                    </Modal.Footer>
                </Modal>
            </div>
        )
    }
}