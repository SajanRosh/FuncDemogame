import logo from './logo.svg';
import './App.css';
import { useEffect } from 'react';
import axios from 'axios'

function App() {
  const url = "/api/Function1";
  useEffect(() => {
    axios.get(url).then(res => {
      console.log(res.data)
    })
  }, [])
  const favWrestler = process.env.REACT_APP_FAV_WRESTLER;
  console.log("Favorite Wrestler from react game,....:", favWrestler);
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
    </div>
  );
}

export default App;
