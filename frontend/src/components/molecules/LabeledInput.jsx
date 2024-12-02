import React from 'react';
import Input from '../atoms/Input';
import Icon from '../atoms/Icon';
import Label from '../atoms/Label';
import '../styles/LabeledInput.css';

const LabeledInput = ({ icon, label, ...props }) => (
  <div className="labeled-input">
    <div className="input-label">
      <Icon icon={icon} className="input-icon" />
      <Label>{label}</Label>
    </div>
    <Input {...props} />
  </div>
);

export default LabeledInput;
