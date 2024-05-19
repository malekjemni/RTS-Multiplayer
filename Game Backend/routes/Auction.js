import express from 'express';
import { body, param } from 'express-validator';
import {
    createAuction,
    deleteAuction,
    getAllAuctions,
    GetAuction,
    CompleteTrade,
    GetPlayerAuctions

} from '../controllers/Auction.js'; 

const router = express.Router();

router.route('/auction/create')
.post(
    body('seller').isMongoId().withMessage('Seller must be a valid ID.'),
    body('sellerResourceType').isString().notEmpty().withMessage('Seller resource type is required.'),
    body('sellerAmount').isNumeric().withMessage('Seller amount must be a number.'),
    body('buyerResourceType').isString().notEmpty().withMessage('Buyer resource type is required.'),
    body('buyerAmount').isNumeric().withMessage('Buyer amount must be a number.'),
    body('state').isBoolean().withMessage('State must be a boolean.'),
    createAuction
)


router.delete('/auction/:auctionId', deleteAuction)
router.get('/auctions', getAllAuctions)
router.get('/auction/:auctionId', GetAuction)

router.route('/auction/:auctionId')
.put(
        body('state').optional().isBoolean(),
        CompleteTrade
)

router.get('/playerAuctions/:seller',GetPlayerAuctions)

export default router;