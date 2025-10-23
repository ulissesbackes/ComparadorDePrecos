import React from 'react';
import SearchProducts from './components/SearchProducts';
import './App.css';

function App() {
  return (
    <div className="App">
      <header className="app-header">
         <h1>🛒 Comparador de Preços</h1>
        <p>Encontre os melhores preços nos seus mercados favoritos</p>
      </header>
      
      <main>
        <SearchProducts />
      </main>
    </div>
  );
}

export default App;