import { fetch, addTask } from 'domain-task';
import { Action, Reducer, ActionCreator } from 'redux';
import { AppThunkAction } from './';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface CounterState {
    count: number;
    createdAt: Date;
}

export interface CounterData {
    count: number;
    createdAt: Date;
}
// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
// Use @typeName and isActionType for type detection that works even after serialization/deserialization.

interface IncrementCountAction { type: 'INCREMENT_COUNT' }
interface DecrementCountAction { type: 'DECREMENT_COUNT' }
interface GetAction { type: 'GET_DATA', data: CounterData }
interface PostAction { type: 'POST_DATA', data: CounterData }

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = IncrementCountAction | DecrementCountAction | GetAction | PostAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    increment: () => <IncrementCountAction>{ type: 'INCREMENT_COUNT' },
    decrement: () => <DecrementCountAction>{ type: 'DECREMENT_COUNT' },
    get: (): AppThunkAction<KnownAction> => (dispatch) => {
        // Only load data if it's something we don't already have (and are not already loading)
        let fetchTask = fetch(`api/SampleData`)
            .then(response => response.json() as Promise<CounterData>)
            .then(data => {
                dispatch({ type: 'GET_DATA', data: data });
            });

        addTask(fetchTask); // Ensure server-side prerendering waits for this to complete
    },
    post: (input: CounterData): AppThunkAction<KnownAction> => (dispatch) => {
        // Only load data if it's something we don't already have (and are not already loading)
        let postTask = fetch(`api/SampleData`, {
            method: "POST",
            headers: { 'content-type': 'application/json;charset=UTF-8' },
            body: JSON.stringify(input)
        })
            .then(response => response.json() as Promise<CounterData>)
            .then(data => {
                dispatch({ type: 'POST_DATA', data: data });
            });

        addTask(postTask); // Ensure server-side prerendering waits for this to complete
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

export const reducer: Reducer<CounterState> = (state: CounterState, action: KnownAction) => {
    switch (action.type) {
        case 'INCREMENT_COUNT':
            return { count: state.count + 1, createdAt: new Date() };
        case 'DECREMENT_COUNT':
            return { count: state.count - 1, createdAt: new Date() };
        case 'GET_DATA':
        case 'POST_DATA':
            return { count: action.data.count, createdAt: action.data.createdAt };
        default:
            // The following line guarantees that every action in the KnownAction union has been covered by a case above
            const exhaustiveCheck: never = action;
    }

    // For unrecognized actions (or in cases where actions have no effect), must return the existing state
    //  (or default initial state if none was supplied)
    return state || { count: 0 };
};
