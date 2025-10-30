import logo from "./logo.svg";
import "./App.css";
import { useState } from "react";
import axios from "axios";

function App() {
  //const url = "/api/Function1";
  
  const [status, setStatus] = useState("");
  const [count, setCount] = useState(1);
  const [message, setMessage] = useState({id : count, details : ""});

  // useEffect(() => {
  //   axios.get("/api/Function1").then((res) => {
  //     console.log(res.data);
  //   });
  // }, []);
  //const favWrestler = process.env.REACT_APP_FAV_WRESTLER;
  //console.log("Favorite Wrestler from react game,....:", favWrestler);

  const handleSend = async () => {
  setStatus("Sending...");

  try {
    const functionUrl = "/api/SendToServiceBus";

    // Send the actual JSON directly
    const res = await axios.post(
      functionUrl,
      { id: count.toString(), details: message.details },
      { headers: { "Content-Type": "application/json" } }
    );

    console.log("Function response:", res.data);
    setStatus("✅ Message sent successfully!");
    setCount(count + 1);
  } catch (err) {
    console.error("Error sending message:", err);
    setStatus("❌ Failed to send message. Check console.");
  }
  }
  function handleMessage(newMessage){
    setMessage({id : count, details : newMessage});
    setCount(count + 1);
  }

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
      <div style={{ maxWidth: 400, margin: "2rem auto", textAlign: "center" }}>
        <h2>Send Message to Queue</h2>
        <textarea
          rows="4"
          cols="40"
          placeholder="Type your message..."
          value={message.details}
          onChange={(e) => handleMessage(e.target.value)}
          style={{ width: "100%", padding: "10px" }}
        />
        <br />
        <button
          onClick={handleSend}
          style={{
            marginTop: "10px",
            padding: "10px 20px",
            backgroundColor: "#0078D4",
            color: "#fff",
            border: "none",
            cursor: "pointer",
          }}
        >
          Send
        </button>
        <p>{status}</p>
      </div>
    </div>
  );
}

export default App;
