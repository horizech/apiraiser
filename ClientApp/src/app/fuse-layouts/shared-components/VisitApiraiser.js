import Button from "@mui/material/Button";
import Icon from "@mui/material/Icon";
import clsx from "clsx";

function VisitApiraiser({ className }) {
    return (
        <Button
            component="a"
            href="https://apiraiser.com"
            target="_blank"
            rel="noreferrer noopener"
            role="button"
            className={clsx("", className)}
            variant="contained"
            color="secondary"
        >
            <Icon className="text-16">link</Icon>
            <span className="mx-4">Visit Apiraiser</span>
        </Button>
    );
}

export default VisitApiraiser;
