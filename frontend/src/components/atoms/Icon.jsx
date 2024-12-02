import React from 'react';

const Icon = ({ icon: IconComponent, ...props }) => (
  <IconComponent {...props} />
);

export default Icon;
