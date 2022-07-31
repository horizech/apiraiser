import { yupResolver } from "@hookform/resolvers/yup";
import TextField from "@mui/material/TextField";
import Button from "@mui/material/Button";
import Icon from "@mui/material/Icon";
import IconButton from "@mui/material/IconButton";
import InputAdornment from "@mui/material/InputAdornment";
import Typography from "@mui/material/Typography";
import Card from "@mui/material/Card";
import { styled, darken } from "@mui/material/styles";
import CardContent from "@mui/material/CardContent";
import { motion } from "framer-motion";
import { Link } from "react-router-dom";
import * as yup from "yup";
import _ from "@lodash";
import { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import apiraiser from "./ApiraiserSlice";
import { CircularProgress } from "@mui/material";

const Root = styled("div")(({ theme }) => ({
    background: `linear-gradient(to right, ${
        theme.palette.primary.dark
    } 0%, ${darken(theme.palette.primary.dark, 0.5)} 100%)`,
    color: theme.palette.primary.contrastText,

    "& .InitializeApiraiser-leftSection": {},

    "& .InitializeApiraiser-rightSection": {
        background: `linear-gradient(to right, ${
            theme.palette.primary.dark
        } 0%, ${darken(theme.palette.primary.dark, 0.5)} 100%)`,
        color: theme.palette.primary.contrastText,
    },
}));

const InitializeApiraiser = () => {
    const dispatch = useDispatch();

    /**
     * Form Validation Schema
     */
    const schema = yup.object().shape({
        username: yup
            .string()
            .min(6, "Username is too short - should be 6 chars minimum.")
            .required("You must enter a username"),
        email: yup
            .string()
            .email("You must enter a valid email")
            .required("You must enter a email"),
        password: yup
            .string()
            .required("Please enter your password.")
            .min(4, "Password is too short - should be 4 chars minimum."),
    });

    const defaultValues = {
        username: "",
        email: "",
        password: "",
    };

    const {
        control,
        setValue,
        formState,
        handleSubmit,
        reset,
        trigger,
        setError,
    } = useForm({
        mode: "onChange",
        defaultValues,
        resolver: yupResolver(schema),
    });

    const { isValid, dirtyFields, errors } = formState;

    const [showPassword, setShowPassword] = useState(false);

    const isInitializedState = useSelector(
        ({ apiraiser }) => apiraiser.isInitializedState
    );

    const initializeState = useSelector(
        ({ apiraiser }) => apiraiser.initializeState
    );

    const initializeResult = useSelector(
        ({ apiraiser }) => apiraiser.initializeResult
    );

    const [isInitializing, setIsInitializing] = useState(false);

    useEffect(() => {
        if (initializeState === "loading") {
            setIsInitializing(true);
        } else if (
            (initializeState === "success" || initializeState === "error") &&
            isInitializing
        ) {
            setIsInitializing(false);
            setTimeout(() => history.push("/"), 0);
        }
    }, [initializeState]);

    const onSubmit = (model) => {
        const postData = {
            Username: model.username,
            Email: model.email,
            Password: model.password,
        };
        dispatch(apiraiser.thunks.initialize(postData));
    };

    return (
        <Root className="flex flex-col flex-auto items-center justify-center shrink-0 p-16 md:p-24">
            <motion.div
                initial={{ opacity: 0, scale: 0.6 }}
                animate={{ opacity: 1, scale: 1 }}
                className="flex w-full max-w-400 md:max-w-3xl rounded-20 shadow-2xl overflow-hidden"
            >
                <Card
                    className="InitializeApiraiser-leftSection flex flex-col w-full max-w-sm items-center justify-center shadow-0"
                    square
                >
                    <CardContent className="flex flex-col items-center justify-center w-full py-96 max-w-320">
                        <motion.div
                            initial={{ opacity: 0 }}
                            animate={{ opacity: 1, transition: { delay: 0.2 } }}
                        >
                            <div className="flex items-center mb-48">
                                <img
                                    className="logo-icon w-48"
                                    src="assets/images/logos/apiraiser.svg"
                                    alt="logo"
                                />
                                <div className="border-l-1 mr-4 w-1 h-40" />
                                <div>
                                    <Typography
                                        className="text-24 font-semibold logo-text"
                                        color="inherit"
                                    >
                                        Apiraiser
                                    </Typography>
                                    <Typography
                                        className="text-16 tracking-widest -mt-8 font-700"
                                        color="textSecondary"
                                    >
                                        CMS
                                    </Typography>
                                </div>
                            </div>
                        </motion.div>
                        <div className="w-full">
                            <form
                                className="flex flex-col justify-center w-full"
                                onSubmit={handleSubmit(onSubmit)}
                            >
                                <Controller
                                    name="username"
                                    control={control}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            className="mb-16"
                                            type="text"
                                            error={!!errors.username}
                                            helperText={
                                                errors?.username?.message
                                            }
                                            label="Username"
                                            InputProps={{
                                                endAdornment: (
                                                    <InputAdornment position="end">
                                                        <Icon
                                                            className="text-20"
                                                            color="action"
                                                        >
                                                            user
                                                        </Icon>
                                                    </InputAdornment>
                                                ),
                                            }}
                                            variant="outlined"
                                        />
                                    )}
                                />

                                <Controller
                                    name="email"
                                    control={control}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            className="mb-16"
                                            type="text"
                                            error={!!errors.email}
                                            helperText={errors?.email?.message}
                                            label="Email"
                                            InputProps={{
                                                endAdornment: (
                                                    <InputAdornment position="end">
                                                        <Icon
                                                            className="text-20"
                                                            color="action"
                                                        >
                                                            user
                                                        </Icon>
                                                    </InputAdornment>
                                                ),
                                            }}
                                            variant="outlined"
                                        />
                                    )}
                                />

                                <Controller
                                    name="password"
                                    control={control}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            className="mb-16"
                                            label="Password"
                                            type="password"
                                            error={!!errors.password}
                                            helperText={
                                                errors?.password?.message
                                            }
                                            variant="outlined"
                                            InputProps={{
                                                className: "pr-2",
                                                type: showPassword
                                                    ? "text"
                                                    : "password",
                                                endAdornment: (
                                                    <InputAdornment position="end">
                                                        <IconButton
                                                            onClick={() =>
                                                                setShowPassword(
                                                                    !showPassword
                                                                )
                                                            }
                                                            size="large"
                                                        >
                                                            <Icon
                                                                className="text-20"
                                                                color="action"
                                                            >
                                                                {showPassword
                                                                    ? "visibility"
                                                                    : "visibility_off"}
                                                            </Icon>
                                                        </IconButton>
                                                    </InputAdornment>
                                                ),
                                            }}
                                            required
                                        />
                                    )}
                                />

                                <Button
                                    disabled={
                                        isInitializing ||
                                        _.isEmpty(dirtyFields) ||
                                        !isValid
                                    }
                                    type="submit"
                                    variant="contained"
                                    color="primary"
                                    className="w-full mx-auto mt-16"
                                >
                                    {isInitializing && (
                                        <CircularProgress size={"2rem"} />
                                    )}
                                    Initialize
                                </Button>
                            </form>
                        </div>
                    </CardContent>
                </Card>

                <div className="InitializeApiraiser-rightSection hidden md:flex flex-1 items-center justify-center p-64">
                    <div className="max-w-320">
                        <motion.div
                            initial={{ opacity: 0, y: 40 }}
                            animate={{
                                opacity: 1,
                                y: 0,
                                transition: { delay: 0.2 },
                            }}
                        >
                            <Typography
                                variant="h3"
                                color="inherit"
                                className="font-semibold leading-tight"
                            >
                                Welcome <br />
                                to the <br /> Apiraiser CMS!
                            </Typography>
                        </motion.div>

                        <motion.div
                            initial={{ opacity: 0 }}
                            animate={{ opacity: 1, transition: { delay: 0.3 } }}
                        >
                            <Typography
                                variant="subtitle1"
                                color="inherit"
                                className="mt-32"
                            >
                                Powerful and professional CMS for multipurpose
                                applications.
                            </Typography>
                        </motion.div>
                    </div>
                </div>
            </motion.div>
        </Root>
    );
};

export default InitializeApiraiser;
