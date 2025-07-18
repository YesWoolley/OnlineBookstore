import React from 'react';

// Template for creating new React components
interface ComponentTemplateProps {
  // Add your props here
  title?: string;
  children?: React.ReactNode;
}

const ComponentTemplate: React.FC<ComponentTemplateProps> = ({ 
  title = 'Default Title',
  children 
}) => {
  // Add your state here
  // const [state, setState] = useState();

  // Add your effects here
  // useEffect(() => {
  //   // Effect logic
  // }, []);

  // Add your handlers here
  const handleClick = () => {
    // Handle click logic
    console.log('Component clicked');
  };

  return (
    <div className="component-template">
      <h2>{title}</h2>
      <div className="content">
        {children}
      </div>
      <button onClick={handleClick}>
        Click me
      </button>
    </div>
  );
};

export default ComponentTemplate; 