import React, { Component } from 'react'
import Drop from 'react-dropzone'
import uploadIcon from '../assets/uploadSmall.png'

 export default class Dropzone extends Component {
    render() {
        let onDrop = this.props.onDrop
        let accept = this.props.accept
        let text = this.props.text

        return (
            <div className="root-dropzone">
                <Drop
                    accept={accept}
                    onDrop={onDrop}
                >
                    {({ getRootProps, getInputProps }) => (
                        <div {...getRootProps()} style={{ backgroundColor: this.props.transition ? '#42ba9e' : '' }} className="dropzone">
                            <input {...getInputProps()} />
                            <div className="column" style={{ marginLeft: 'auto', marginRight: 'auto' }}>
                                <div className="row">
                                    {text}
                                </div>
                                <div style={{ textAlign:'center' }}>
                                    <img  width="50" height="50" src={uploadIcon} alt="Logo" />
                                </div>
                            </div>
                        </div>
                    )}
                </Drop>
            </div>
            )
    }
}
