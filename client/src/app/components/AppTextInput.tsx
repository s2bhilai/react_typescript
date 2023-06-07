import { TextField } from "@mui/material";
import { UseControllerProps, useController } from "react-hook-form";

interface Props extends UseControllerProps {
    label: String;
}

export default function AppTextInput(props: Props) {
    const { fieldState, field } = useController({ ...props, defaultValue: '' });
    
    return (
        <TextField
            { ...props }
            { ...field }
            fullWidth
            variant="outlined"
            error={!!fieldState.error}
            helperText={fieldState.error?.message}
        />
    );
}