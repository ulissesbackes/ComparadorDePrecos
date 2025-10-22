import React from 'react';
import './ProductCard.css';

const ProductCard = ({ product }) => {
  const handleViewDetails = () => {
    if (product.url) {
      window.open(product.url, '_blank');
    }
  };

  const handleAddToList = () => {
    // TODO: Integrar com funcionalidade de lista de compras
    alert(`"${product.nome}" adicionado à lista de compras!\nPreço: R$ ${product.preco.toFixed(2)}\nMercado: ${product.mercado}`);
  };

  return (
    <div className="product-card">
      <div className="product-image-container">
        <img 
          src={product.imagem || '/placeholder-image.jpg'} 
          alt={product.nome}
          className="product-image"
          onError={(e) => {
            e.target.src = '/placeholder-image.jpg';
          }}
        />
        <span className="product-market-badge">{product.mercado}</span>
      </div>
      
      <div className="product-info">
        <h3 className="product-name">{product.nome}</h3>
        
        <div className="product-pricing">
          <span className="product-price">R$ {product.preco.toFixed(2)}</span>
          {product.precoOriginal && product.precoOriginal > product.preco && (
            <span className="product-original-price">
              R$ {product.precoOriginal.toFixed(2)}
            </span>
          )}
        </div>
        
        <div className="product-actions">
          <button 
            className="btn btn-secondary"
            onClick={handleViewDetails}
          >
            Ver Detalhes
          </button>
          <button 
            className="btn btn-primary"
            onClick={handleAddToList}
          >
            + Lista
          </button>
        </div>
      </div>
    </div>
  );
};

export default ProductCard;