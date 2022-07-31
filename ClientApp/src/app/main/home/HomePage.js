import Typography from "@mui/material/Typography";
import { motion } from "framer-motion";

function HomePage() {
    return (
        <div className="flex flex-col flex-1 items-center justify-center p-16">
            <div className="max-w-512 text-center">
                <motion.div
                    initial={{ opacity: 0, scale: 0.6 }}
                    animate={{
                        opacity: 1,
                        scale: 1,
                        transition: { delay: 0.1 },
                    }}
                >
                    <Typography
                        variant="h1"
                        color="inherit"
                        className="font-medium mb-16"
                    >
                        Home
                    </Typography>
                </motion.div>

                <motion.div
                    initial={{ opacity: 0, y: 40 }}
                    animate={{ opacity: 1, y: 0, transition: { delay: 0.2 } }}
                >
                    <Typography
                        variant="h5"
                        color="textSecondary"
                        className="mb-16 font-normal"
                    >
                        Welcome to Apiraiser CMS
                    </Typography>
                </motion.div>
            </div>
        </div>
    );
}

export default HomePage;
