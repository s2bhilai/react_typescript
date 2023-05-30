import { Checkbox, FormControlLabel, FormGroup } from "@mui/material";
import { useState } from "react";

interface Props {
  items: string[];
  checked?: string[];
  onChange: (items: string[]) => void;
}

export default function CheckBoxButtons({ items, checked, onChange }: Props) {
  const [checkedItems, setCheckedItems] = useState(checked || []);

  function handleChecked(value: string) {
    const currentIndex = checkedItems.findIndex((item) => item === value);
    let newChecked: string[] = [];
    if (currentIndex === -1) newChecked = [...checkedItems, value]; //new item to be added to array
    else newChecked = checkedItems.filter((item) => item != value);//returns checked items - unchecked item

    setCheckedItems(newChecked);
    onChange(newChecked);
  }

  return (
    <FormGroup>
      {items.map((item) => {
        return (
          <FormControlLabel
            control={<Checkbox checked={checkedItems.indexOf(item) !== -1} />}
            onClick={() => handleChecked(item)}
            label={item}
            key={item}
          />
        );
      })}
    </FormGroup>
  );
}
