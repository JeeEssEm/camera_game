using System.Collections.Generic;

namespace Commands
{
    public class CommandManager : ICommandManager
    {
        private Stack<BaseCommand> _undoStack = new();
        private Stack<BaseCommand> _redoStack = new();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// ёзать менеджер только дл€ команд, которые умеют в Undo!
        /// </summary>
        /// <param name="command"></param>
        public void Execute(BaseCommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var cmd = _redoStack.Pop();
                cmd.Execute();
                _undoStack.Push(cmd);
            }
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var cmd = _undoStack.Pop();
                cmd.Undo();
                _redoStack.Push(cmd);
            }

        }
    }
}
