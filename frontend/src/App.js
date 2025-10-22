import React, { useState } from 'react';
import './App.css';
import SearchProducts from './components/SearchProducts';

function App() {
  return (
    <div className="App">
      <header className="app-header">
        <div className="header-content">
          <h1>🛒 Comparador de Preços</h1>
          <p>Encontre os melhores preços nos seus mercados favoritos</p>
        </div>
      </header>
      
      <main className="main-content">
        <SearchProducts />
      </main>
      
      <footer className="app-footer">
        <p>© 2024 Comparador de Preços - Encontre as melhores ofertas</p>
      </footer>
    </div>
  );
}

export default App;