import express from "express";
import { body } from "express-validator";
import { addPlayer , getPlayer,  login, updatePlayer,getAll,getOnce,getUserbyUserName,getPlayerName,getLeaderboard } from "../controllers/Player.js";


const router = express.Router();

router.route('/addplayer')
.post(
    body("username").isLength({min : 5 , max  : 16 }),
    body("password").isLength({min : 5 , max  : 20 }),
    addPlayer
)
router.route('/players/:username')
    .put(
        body("username"),
        body("password"),
        body("score"),
        body("gold"),
        body("storagewood"),
        body("storagemud"),
        body("storageclay"),
        body("storageenergie"),
        body("MaxWood"),
        body("MaxMud"),
        body("MaxClay"),
        body("MaxEnergie"),
        body("wood"),
        body("iron"),
        body("mud"),
        body("solar"),
        body("wind"),
        body("water"),
        updatePlayer
    )

router.route('/getUserbyUserId/:UseName').get(getUserbyUserName);

router.route('/player')
.get(
    body("username"),
    getPlayer
)

router.route('/')
    .get(getAll);


router
    .route('/login')
    .post(login);


router.route('/getOnce/:id').get(getOnce)

// Corrected route parameter to match the one used in the controller
router.route('/getUserbyUserName/:UserName').get(getUserbyUserName);

router.get('/player/:playerId', getPlayerName)
router.get('/leaderboard', getLeaderboard)
export default router;