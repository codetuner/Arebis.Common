using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

using System.Windows.Forms;
using System.IO;

namespace Arebis.Windows.Forms {

	public enum TextBoxStreamerBinding {
		None = 0,
        StandardOut,
		StandardErr
	}

	public class TextBoxStreamer : System.ComponentModel.Component {

		private TextBoxWriter writer = new TextBoxWriter();
		private TextBoxStreamerBinding binding = TextBoxStreamerBinding.None;

		public TextBoxStreamer() {
		}

		[Description("Textbox where redirected stream should be outputted to."),
		DefaultValue(null)]
		public System.Windows.Forms.TextBoxBase TextBox {
			get {
				return this.writer.Component;
			}
			set {
				this.writer.Component = value;
			}
		}

		[Description("Textbox where redirected stream should be outputted to."),
		DefaultValue(null)]
		public bool AutoMoveSelection {
			get {
				return this.writer.AutoMoveSelection;
			}
			set {
				this.writer.AutoMoveSelection = value;
			}
		}

		[Description("To which output stream this component streamer should be bound."),
		DefaultValue(TextBoxStreamerBinding.None)]
		public Arebis.Windows.Forms.TextBoxStreamerBinding Binding {
			get {
				return this.binding;
			}
			set {
				switch(this.binding) {
					case TextBoxStreamerBinding.StandardOut: 
						Console.SetOut(writer.OriginalWriter);
						break;
					case TextBoxStreamerBinding.StandardErr:
						Console.SetError(writer.OriginalWriter);
						break;
				}
				this.binding = value;
				switch(this.binding) {
					case TextBoxStreamerBinding.None: 
						writer.OriginalWriter = null;
						break;
					case TextBoxStreamerBinding.StandardOut: 
						writer.OriginalWriter = Console.Out;
						Console.SetOut(this.writer);
						break;
					case TextBoxStreamerBinding.StandardErr:
						writer.OriginalWriter = Console.Error;
						Console.SetError(this.writer);
						break;
				}
			}
		}

		[Description("Echo on original stream as well ?"),
		DefaultValue(false)]
		public bool Echo {
			get {
				return this.writer.Echo;
			}
			set {
				this.writer.Echo = value;
			}
		}


		public System.IO.TextWriter Writer {
			get {
				return this.writer;
			}
		}

		
		internal class TextBoxWriter : TextWriter {

			private TextBoxBase component;
			private TextWriter originalWriter = null;
			private bool autoMoveSelection = true;
			private bool echo = false;

			internal System.Windows.Forms.TextBoxBase Component {
				get {
					return this.component;
				}
				set {
					this.component = value;
				}
			}

			internal System.IO.TextWriter OriginalWriter {
				get {
					return this.originalWriter;
				}
				set {
					this.originalWriter = value;
				}
			}

			internal bool AutoMoveSelection {
				get {
					return this.autoMoveSelection;
				}
				set {
					this.autoMoveSelection = value;
				}
			}

			internal bool Echo {
				get {
					return this.echo;
				}
				set {
					this.echo = value;
				}
			}

			public override System.Text.Encoding Encoding {
				get {
					return null;
				}
			}
	
			public override void Write(char[] buffer, int index, int count) {
				if (echo) this.originalWriter.Write(buffer, index, count);
				if (component == null) return;
				System.Text.StringBuilder sb = new System.Text.StringBuilder(buffer.Length);
				sb.Append(buffer);
				component.AppendText(sb.ToString());
				MoveSelection();
			}
	
			public override void Write(char value) {
				if (echo) this.originalWriter.Write(value);
				if (component == null) return;
				component.Text += value;
				MoveSelection();
			}
	
			public override void Write(string value) {
				if (echo) this.originalWriter.Write(value);
				if (component == null) return;
				component.Text += value;
				MoveSelection();
			}

			protected virtual void MoveSelection() {
				if (!autoMoveSelection) return;
				this.component.SelectionStart = this.component.Text.Length-1;
				this.component.SelectionLength = 0;
			}
		}
	}
}
