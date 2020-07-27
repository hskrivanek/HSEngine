using HSEngine.Core.InputSystem;
using Moq;
using System;
using System.Collections.Generic;
using System.Numerics;
using Veldrid;
using Xunit;

namespace HSEngine.Core.Tests
{
    public class InputTests
    {
        [Fact]
        public void GetKeyReturnsTrueForNewlyPressedKey()
        {
            var snapshotMock = new Mock<InputSnapshot>();
            UpdateFrameInputWithPressedKey(Key.A);

            Assert.True(Input.GetKey(Key.A));
        }

        [Fact]
        public void GetKeyReturnsTrueForKeyPressedInPreviousFrame()
        {
            UpdateFrameInputWithPressedKey(Key.B);
            UpdateFrameInputWithNoEvents();

            Assert.True(Input.GetKey(Key.B));
        }

        [Fact]
        public void GetKeyReturnsFalseAfterKeyRelease()
        {
            UpdateFrameInputWithPressedKey(Key.C);
            UpdateFrameInputWithReleasedKey(Key.C);

            Assert.True(Input.GetKey(Key.C));
        }

        [Fact]
        public void GetKeyDownReturnsTrueForNewlyPressedKey()
        {
            UpdateFrameInputWithPressedKey(Key.D);

            Assert.True(Input.GetKeyDown(Key.D));
        }

        [Fact]
        public void GetKeyDownReturnsFalseForKeyPressedInPreviousFrame()
        {
            UpdateFrameInputWithPressedKey(Key.E);
            UpdateFrameInputWithNoEvents();

            Assert.False(Input.GetKeyDown(Key.E));
        }

        private static void UpdateFrameInputWithPressedKey(Key key)
        {
            var snapshotMock = new Mock<InputSnapshot>();

            snapshotMock.Setup(s => s.KeyEvents).Returns(new KeyEvent[] { new KeyEvent(key, true, ModifierKeys.None) });
            Input.UpdateFrameInput(snapshotMock.Object);
        }

        private static void UpdateFrameInputWithReleasedKey(Key key)
        {
            var snapshotMock = new Mock<InputSnapshot>();

            snapshotMock.Setup(s => s.KeyEvents).Returns(new KeyEvent[] { new KeyEvent(key, true, ModifierKeys.None) });
            Input.UpdateFrameInput(snapshotMock.Object);
        }

        private static void UpdateFrameInputWithNoEvents()
        {
            var snapshotMock = new Mock<InputSnapshot>();

            snapshotMock.Setup(s => s.KeyEvents).Returns(new KeyEvent[] { });
            Input.UpdateFrameInput(snapshotMock.Object);
        }
    }
}
