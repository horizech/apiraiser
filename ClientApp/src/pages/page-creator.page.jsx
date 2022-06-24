import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { withRouter } from "react-router-dom";

import { history } from '../helpers';
import { Page } from '../components';

import ReactDOM from "react-dom";
import debounce from "debounce";
import { LiveEditor } from '../components';

import '../styles/PageCreator.css';

// default code 
const code = `
// import React from 'react';
// import { Link } from 'react-router-dom';
// import { connect } from 'react-redux';
// import * as Acorn from "acorn";

// const ast = Acorn.parse("", {
// 	sourceType: "module"
// });


// edit this example
let styles = {
    backgroundColor : 'red',
    fill: 'blue',
    width: '32px',
    height: '50%'
};

function PageCreator() {
    return (
        <div className="row">
            <div className="col-md-4">
                <span style={styles}>Hello World!</span>
            </div>
            <div className="col-md-4">
                <span style={styles}>Hello World!</span>
            </div>
            <div className="col-md-4">
                <span style={styles}>Hello World!</span>
            </div>
        </div>
    );
}


function mapStateToProps(state) {
    const { loggedIn } = state.authentication;
    const { loading, table } = state.table;
    return {
        loading,
        table,
        loggedIn,
    };
}

const Page = connect(mapStateToProps)(PageCreator);

<Page />
`;

class PageCreatorPage extends Page {
    state = {
        superError: null,
        code
    };

	editor = null;

	el = null;
  
	componentDidMount() {
		this.editor = LiveEditor(this.el);
		this.editor.run(code);
	}

	onCodeChange = ({ target: { value } }) => {
		this.setState({ code: value });
		this.run(value);
	};

	run = debounce(() => {
		const { code } = this.state;
		try{
			this.editor.run(code);
			this.setState( { superError: null});
		}
		catch(e) {
			this.setState( { superError: e.message});
		}
	}, 500);

	render() {
		const { code, superError } = this.state;
		return (
		<div className="app">
			<div className="split-view">
			<div className="code-editor">
				<textarea className="code-editor-textarea" value={code} onChange={this.onCodeChange} />
			</div>
			<div className="preview" ref={el => (this.el = el)} />
			</div>
		</div>
		);
	}
}

function mapStateToProps(state) {
    const { loggedIn } = state.authentication;
    const { loading, table } = state.table;
    return {
        loading,
        table,
        loggedIn,
    };
}

const connectedPageCreator = connect(mapStateToProps)(PageCreatorPage);
export { connectedPageCreator as PageCreatorPage };
