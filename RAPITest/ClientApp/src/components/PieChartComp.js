import React, { Component } from 'react'
import { PieChart, Pie, Tooltip, Cell, ResponsiveContainer } from 'recharts';

const RADIAN = Math.PI / 180;
const renderCustomizedLabel = ({ cx, cy, midAngle, innerRadius, outerRadius, percent, index }) => {
    const radius = innerRadius + (outerRadius - innerRadius) * 0.5;
    const x = cx + radius * Math.cos(-midAngle * RADIAN);
    const y = cy + radius * Math.sin(-midAngle * RADIAN);

    return (
        <text x={x} y={y} fill="white" textAnchor={x > cx ? 'start' : 'end'} dominantBaseline="central">
            {`${(percent * 100).toFixed(0)}%`}
        </text>
    );
};

export default class PieChartComp extends Component {

    render() {
        let width = this.props.width
        let height = this.props.height
        let data = this.props.data
        let cx = this.props.cx
        let cy = this.props.cy
        let outerRadius = this.props.outerRadius
        let dataKey = this.props.dataKey
        let COLORS = this.props.colors

        return (

         
                <PieChart width={width} height={height}>
                    <Pie
                        data={data}
                        cx={cx}
                        cy={cy}
                        labelLine={false}
                        label={renderCustomizedLabel}
                        outerRadius={outerRadius}
                        dataKey={dataKey}
                        stroke="black"
                    >
                        {data.map((entry, index) => (
                            <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                        ))}
                    </Pie>
                    <Tooltip />
                </PieChart>

        );
    }
}
