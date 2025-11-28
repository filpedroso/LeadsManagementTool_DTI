import React from 'react'
import { ToastContainer } from 'react-toastify'
import 'react-toastify/dist/ReactToastify.css'
import { Header } from './components/common/Header'
import { LeadsContainer } from './components/leads/LeadsContainer'
import './App.css'

function App() {
  return (
    <div className="app">
      <Header />
      <div className="container">
        <LeadsContainer />
      </div>
      <ToastContainer
        position="bottom-right"
        autoClose={3000}
        hideProgressBar={false}
        newestOnTop={true}
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
      />
    </div>
  )
}

export default App
