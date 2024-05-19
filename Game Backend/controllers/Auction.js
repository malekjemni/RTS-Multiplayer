import AuctionModel from '../models/Auction.js';

export async function createAuction(req, res) {
    try {
        const { seller, sellerResourceType, sellerAmount, buyerResourceType, buyerAmount, state } = req.body;

        const newAuction = new AuctionModel({
            seller,
            sellerResourceType,
            sellerAmount,
            buyerResourceType,
            buyerAmount,
            state,
        });

        const savedAuction = await newAuction.save();
        res.status(201).json(savedAuction);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'An error occurred while creating the auction.' });
    }
}

export async function deleteAuction(req, res) {
    try {
        const { auctionId } = req.params;
        await AuctionModel.findByIdAndDelete(auctionId);

        res.status(200).json({ message: 'Auction deleted successfully.' });
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'An error occurred while deleting the auction.' });
    }
}
export async function getAllAuctions(req, res) {
    try {
        const auctions = await AuctionModel.find();

        if (!auctions || auctions.length === 0) {
            return res.status(200).json({ });
        }

        res.status(200).json({ auctions });
    } catch (error) {
        console.error('Error retrieving auctions:', error);
        res.status(500).json({ error: 'An error occurred while retrieving auctions.' });
    }
}
export async function GetAuction(req, res) {
    try {
        const { auctionId } = req.params;
        const auction = await AuctionModel.findById(auctionId);

        if (!auction) {
            return res.status(404).json({ error: 'auction not found.' });
        }
        res.status(200).json(auction);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'An error occurred while retrieving the auction data.' });
    }
}
export async function CompleteTrade(req, res) {
    try {
        const { auctionId } = req.params;
        const { state } = req.body;

        const updatedAuction = await AuctionModel.findByIdAndUpdate(
            auctionId,
            { state },
            { new: true }
        );

        if (!updatedAuction) {
            return res.status(404).json({ error: 'Auction not found.' });
        }

        res.status(200).json(updatedAuction);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'An error occurred while updating the Auction.' });
    }
}
export async function GetPlayerAuctions(req, res) {
    try {
        const { seller } = req.params;
        const auctions = await AuctionModel.find({ seller });

        if (!auctions || auctions.length === 0) {
            return res.status(200).json({});
        }

        res.status(200).json({auctions:auctions});
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'An error occurred while retrieving the auction data.' });
    }
}