import CellModel from '../models/Cell.js';


export async function createCell(req, res) {
      try {

          const { region, productivite, level, state, index } = req.body;
          const newCell = new CellModel({
              region,
              productivite,
              level,
              state,
              index,
          });
  
          const savedCell = await newCell.save();
          res.status(201).json(savedCell);
      } catch (error) {
          console.error(error);
          res.status(500).json({ error: 'An error occurred while creating the cell.' });
      }
}

export async function updateCell(req, res) {
    try {
        const { cellId } = req.params;
        const { region, productivite, level, state, index } = req.body;

        const updatedCell = await CellModel.findByIdAndUpdate(
            cellId,
            { region, productivite, level, state, index },
            { new: true }
        );

        if (!updatedCell) {
            return res.status(404).json({ error: 'Cell not found.' });
        }

        res.status(200).json(updatedCell);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'An error occurred while updating the cell.' });
    }
}

export async function deleteCell(req, res) {
    try {
        const { cellId } = req.params;
        const deletedCell = await CellModel.findByIdAndRemove(cellId);

        if (!deletedCell) {
            return res.status(404).json({ error: 'Cell not found.' });
        }

        res.status(200).json({ message: 'Cell deleted successfully.' });
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'An error occurred while deleting the cell.' });
    }
}

export async function getCell(req, res) {
    try {
        const { index } = req.params;
        const cellIndex = parseInt(index, 10);
        const cell = await CellModel.findOne({ index: cellIndex });

        if (!cell) {
            return res.status(404).json({ error: 'Cell not found.' });
        }
        res.status(200).json(cell);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'An error occurred while retrieving the cell data.' });
    }
}

export async function getAllCells(req, res) {
    try {
        const cells = await CellModel.find();

        if (!cells || cells.length === 0) {
            return res.status(404).json({ error: 'No cells found.' });
        }

        res.status(200).json({cells});
    } catch (error) {
        console.error('Error retrieving cells:', error);
        res.status(500).json({ error: 'An error occurred while retrieving cells.' });
    }
}

export async function deleteAllCells(req, res) {
    try {
        const deletedResult = await CellModel.deleteMany({});
        res.status(200).json({ message: 'All cells deleted successfully.', deletedCount: deletedResult.deletedCount });
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'An error occurred while deleting the cells.' });
    }
}

