import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

// Template for creating new page components
interface PageTemplateProps {
  title: string;
}

const PageTemplate: React.FC<PageTemplateProps> = ({ title }) => {
  // State management
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [data, setData] = useState<any>(null);

  // Navigation
  const navigate = useNavigate();

  // API call example
  const fetchData = async () => {
    setLoading(true);
    setError(null);
    
    try {
      // Replace with your actual API call
      // const response = await apiService.getData();
      // setData(response.data);
      
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      setData({ message: 'Data loaded successfully' });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  // Load data on component mount
  useEffect(() => {
    fetchData();
  }, []);

  // Handle navigation
  const handleNavigate = (path: string) => {
    navigate(path);
  };

  // Handle form submission
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Add your form submission logic here
    console.log('Form submitted');
  };

  if (loading) {
    return (
      <div className="page-template">
        <div className="loading">Loading...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="page-template">
        <div className="error">
          <h2>Error</h2>
          <p>{error}</p>
          <button onClick={fetchData}>Retry</button>
        </div>
      </div>
    );
  }

  return (
    <div className="page-template">
      <header className="page-header">
        <h1>{title}</h1>
        <button onClick={() => handleNavigate('/')}>
          Back to Home
        </button>
      </header>

      <main className="page-content">
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="example">Example Input:</label>
            <input 
              type="text" 
              id="example" 
              name="example"
              placeholder="Enter something..."
            />
          </div>
          
          <button type="submit">
            Submit
          </button>
        </form>

        {data && (
          <div className="data-display">
            <h3>Data:</h3>
            <pre>{JSON.stringify(data, null, 2)}</pre>
          </div>
        )}
      </main>
    </div>
  );
};

export default PageTemplate; 