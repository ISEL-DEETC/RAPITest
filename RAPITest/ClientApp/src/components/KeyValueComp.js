import React from 'react';
import PropTypes from 'prop-types';
import { Row, Col } from 'react-bootstrap'
import binIcon from '../assets/bin.png'

const c = 'key-value';

export default class KeyValue extends React.Component {
    static displayName = 'KeyValue'

    static propTypes = {
        rows: PropTypes.arrayOf(PropTypes.shape({
            keyItem: PropTypes.string,
            valueItem: PropTypes.string
        })),
        onChange: PropTypes.func,
        customAddButtonRenderer: PropTypes.func,
        keyInputPlaceholder: PropTypes.string,
        valueInputPlaceholder: PropTypes.string,
        hideLabels: PropTypes.bool
    }

    static defaultProps = {
        rows: [],
        onChange: () => { },
        keyInputPlaceholder: '',
        valueInputPlaceholder: '',
        hideLabels: false
    }

    constructor(props) {
        super(props);
        this.state = {
            rows: [
                ...this.props.rows
            ]
        };
    }

    handleAddNew = () => {
        this.setState({
            rows: [
                ...this.state.rows,
                {
                    keyItem: '',
                    valueItem: ''
                }
            ]
        }, () => {
            this.props.onChange([...this.state.rows]);
        });
    }

    handleKeyItemChange(index, value) {
        this.setState({
            rows: this.state.rows.map((row, i) => {
                if (index !== i) {
                    return row;
                }
                return {
                    ...row,
                    keyItem: value
                };
            })
        }, () => {
            this.props.onChange([...this.state.rows]);
        });
    }

    handleValueItemChange(index, value) {
        this.setState({
            rows: this.state.rows.map((row, i) => {
                if (index !== i) {
                    return row;
                }
                return {
                    ...row,
                    valueItem: value
                };
            })
        }, () => {
            this.props.onChange([...this.state.rows]);
        });
    }

    handleRemove(e, index) {
        e.preventDefault();
        let copyArray = this.state.rows
        copyArray.splice(index, 1)

        this.setState({
            rows: copyArray
        }, () => {
            this.props.onChange([...this.state.rows]);
        });
    }

    toJSON() {
        const { rows = [] } = this.state;
        return rows.reduce((acc, row) => {
            acc[row.keyItem] = row.valueItem;
            return acc;
        }, {});
    }

    renderLabelText(text) {
        if (this.props.hideLabels === true) {
            return null;
        }
        return (
            <span>
                {text}
            </span>
        );
    }

    renderKeyItem(index, value) {
        return (
                <input
                    className="InputKeyItem"
                    type="text"
                    value={value}
                    placeholder={this.props.keyInputPlaceholder}
                    onChange={(e) => this.handleKeyItemChange(index, e.currentTarget.value)}
                />
        );
    }

    renderValueItem(index, value) {
        return (
            <input
                    className="InputValueItem"
                    type="text"
                    value={value}
                    placeholder={this.props.valueInputPlaceholder}
                    onChange={(e) => this.handleValueItemChange(index, e.currentTarget.value)}
                />
        );
    }

    renderRows() {
        return this.state.rows.map((row, i) => (
            <Row key={`key-value-row-${i}`}>

                <Col sm={5}>
                    {this.renderKeyItem(i, row.keyItem)}
                </Col>

                <Col sm={5}>
                    {this.renderValueItem(i, row.valueItem)}
                </Col>

                <Col sm={2} style={{marginTop:"5px"}}>
                    <button className="removeButtonBin" onClick={(e) => this.handleRemove(e,i)}>
                        <img width="35" height="35" src={binIcon} alt="LogoBin" />
                    </button>
                </Col>

            </Row>
        ));
    }

    renderAddButton() {
        if (typeof this.props.customAddButtonRenderer === 'function') {
            return this.props.customAddButtonRenderer(this.handleAddNew);
        }
        return (
            <button
                onClick={this.handleAddNew}
            >
                Add new
            </button>
        );
    }

    render() {
        return (
            <div className={c}>
                <div>
                    {this.renderRows()}
                </div>
                <Row>
                    {this.renderAddButton()}
                </Row>
            </div>
        );
    }
}
