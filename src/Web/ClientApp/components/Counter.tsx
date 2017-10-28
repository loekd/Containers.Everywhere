import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as CounterStore from '../store/Counter';
import * as WeatherForecasts from '../store/WeatherForecasts';

type CounterProps =
    CounterStore.CounterState
    & typeof CounterStore.actionCreators
    & RouteComponentProps<{}>;

class Counter extends React.Component<CounterProps, {}> {
    public render() {
        return <div>
            <h1>Persisted Counter</h1>

            
            <h2>Current count: <strong>{this.props.count}</strong></h2>
            <h2>Created at: <strong>{this.props.createdAt}</strong></h2>
            <h2>Stored in: <strong>{this.props.store}</strong></h2>

            <button className="btn" onClick={() => { this.props.get(); }}>Get Counter</button>
            <button className="btn" onClick={() => {
                this.props.post(
                    {
                        count: this.props.count + 1,
                        createdAt: this.props.createdAt,
                        store: "hallo?",
                    });
            }}>Update Counter</button>
        </div>;
    }
}

// Wire up the React component to the Redux store
export default connect(
    (state: ApplicationState) => state.counter, // Selects which state properties are merged into the component's props
    CounterStore.actionCreators                 // Selects which action creators are merged into the component's props
)(Counter) as typeof Counter;