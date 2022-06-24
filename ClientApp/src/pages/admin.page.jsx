import React from "react";
import { Page } from "../components";

export class AdminPage extends Page {
  static displayName = AdminPage.name;

  constructor(props) {
    super(props);
    this.state = { forecasts: [], result: [], loading: true };
  }

  componentDidMount() {
  }

  static renderForecastsTable(forecasts) {
    return (
      <table className="table table-striped" aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Date</th>
            <th>Temp. (C)</th>
            <th>Temp. (F)</th>
            <th>Summary</th>
          </tr>
        </thead>
        <tbody>
          {forecasts.map((forecast) => (
            <tr key={forecast.date}>
              <td>{forecast.date}</td>
              <td>{forecast.temperatureC}</td>
              <td>{forecast.temperatureF}</td>
              <td>{forecast.summary}</td>
            </tr>
          ))}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading ? (
      <p>
        <em>Loading...</em>
      </p>
    ) : (
      AdminPage.renderForecastsTable(this.state.forecasts)
    );

    return (
      <div>
        <h1 id="tabelLabel">Weather forecast</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </div>
    );
  }

  async createTable(schema, table) {
    // const response = await fetch('api/createtable', { method: 'POST', body: { 'schema': schema, 'table': table } });

    // const response = await fetch(`api/createtable/${schema}/${table}`);
    // const data = await response.json();
    // console.log(data);

    this.setState({ loading: false });
  }
}
