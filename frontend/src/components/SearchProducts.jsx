import React, { useState } from 'react';
import ProductCard from './ProductCard';
import './SearchProducts.css';

const SearchProducts = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [selectedMarket, setSelectedMarket] = useState('todos');

  const API_BASE = 'http://localhost:5186'; // Sua API .NET

  const searchProducts = async () => {
    if (!searchTerm.trim()) return;
    
    setLoading(true);
    setError('');
    
    try {
      const response = await fetch(`${API_BASE}/produtos/${encodeURIComponent(searchTerm)}`);
      
      if (!response.ok) {
        throw new Error('Erro ao buscar produtos');
      }
      
      const data = await response.json();
      setProducts(data);
    } catch (err) {
      setError('Erro ao buscar produtos. Tente novamente.');
      console.error('Erro:', err);
    } finally {
      setLoading(false);
    }
  };

  const searchByMarket = async (market) => {
    if (!searchTerm.trim()) return;
    
    setLoading(true);
    setSelectedMarket(market);
    
    try {
      const url = market === 'todos' 
        ? `${API_BASE}/produtos/${encodeURIComponent(searchTerm)}`
        : `${API_BASE}/produtos/${market}/${encodeURIComponent(searchTerm)}`;
      
      const response = await fetch(url);
      const data = await response.json();
      setProducts(data);
    } catch (err) {
      setError('Erro ao buscar produtos.');
    } finally {
      setLoading(false);
    }
  };

  const filteredProducts = selectedMarket === 'todos' 
    ? products 
    : products.filter(p => p.mercado.toLowerCase() === selectedMarket.toLowerCase());

  return (
    <div className="search-products">
      <div className="search-section">
        <div className="search-box">
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            onKeyPress={(e) => e.key === 'Enter' && searchProducts()}
            placeholder="Digite o produto que está procurando..."
            className="search-input"
          />
          <button 
            onClick={searchProducts}
            disabled={loading}
            className="search-button"
          >
            {loading ? '🔍 Buscando...' : '🔍 Buscar'}
          </button>
        </div>

        <div className="market-filters">
          {['todos', 'Angeloni', 'Minhacooper'].map(market => (
            <button
              key={market}
              onClick={() => searchByMarket(market)}
              className={`market-filter ${selectedMarket === market ? 'active' : ''}`}
            >
              {market === 'todos' ? '🌐 Todos' : market}
            </button>
          ))}
        </div>
      </div>

      {error && (
        <div className="error-message">
          ❌ {error}
        </div>
      )}

      {loading && (
        <div className="loading">
          <div className="spinner"></div>
          <p>Buscando produtos...</p>
        </div>
      )}

      {!loading && products.length > 0 && (
        <div className="results-section">
          <div className="results-header">
            <h2>
              {filteredProducts.length} produto(s) encontrado(s) 
              {selectedMarket !== 'todos' && ` em ${selectedMarket}`}
            </h2>
          </div>
          
          <div className="products-grid">
            {filteredProducts.map((product, index) => (
              <ProductCard key={`${product.nome}-${index}`} product={product} />
            ))}
          </div>
        </div>
      )}

      {!loading && products.length === 0 && searchTerm && (
        <div className="empty-state">
          <h3>😕 Nenhum produto encontrado</h3>
          <p>Tente buscar com outros termos</p>
        </div>
      )}
    </div>
  );
};

export default SearchProducts;