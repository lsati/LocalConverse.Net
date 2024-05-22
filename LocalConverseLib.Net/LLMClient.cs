
using LLama.Common;
using LLama;
using System.CodeDom.Compiler;
using System;
using static System.Collections.Specialized.BitVector32;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using LLama.Native;

namespace LocalConverseLib.Net
{
    public class LLMClient : ILLMClient
    {
        private Queue<string> queue = new Queue<string>();
        private readonly SemaphoreSlim semaphore;
        private string userRequest;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        public LLMClient()
        {
            this.semaphore = new SemaphoreSlim(0);
            this.userRequest = string.Empty;
        }

        public void SubmitUserRequest(string userRequest)
        {
            this.userRequest = userRequest;
            this.semaphore.Release();
        }


        public void CancelGeneration()
        {
            this.userRequest = string.Empty;
            this.cancellationTokenSource?.Cancel();
        }

        public async Task StartChatListener(LLMConfig config,
            Action<string> OnModelLoadCompletedCallback,
            Action<string> OnTokenCallback,
            Action<string> OnResponseCompletedCallback)
        {
            var parameters = new ModelParams(config.ModelPath)
            {
                ContextSize = 4098, // The longest length of chat as memory.
                GpuLayerCount = 100, // How many layers to offload to GPU. Please adjust it according to your GPU memory.
            };


            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            var executor = new InteractiveExecutor(context);

            // Add chat histories as prompt to tell AI how to act.
            var chatHistory = new ChatHistory();

            chatHistory.AddMessage(AuthorRole.System, "Assistant is General Purpose AI assistant. Returns response in Markdown format.");
            chatHistory.AddMessage(AuthorRole.Assistant, "start");

            ChatSession session = new(executor, chatHistory);

            InferenceParams inferenceParams = new InferenceParams()
            {
                MaxTokens = 1024, // No more than 256 tokens should appear in answer. Remove it if antiprompt is enough for control.
                AntiPrompts = new List<string> { "\nUser:" }, // Stop generation once antiprompts appear.
                Temperature = 0.1f
            };

            if (OnModelLoadCompletedCallback != null)
                OnModelLoadCompletedCallback(string.Empty);

            StringBuilder sb = new StringBuilder();
            while (true)
            {
                this.cancellationTokenSource = new CancellationTokenSource();
                this.cancellationToken = cancellationTokenSource.Token;

                semaphore.Wait();
                if (OnTokenCallback != null)
                {
                    if (!string.IsNullOrWhiteSpace(this.userRequest))
                    {
                        sb.Clear();
                        await foreach ( // Generate the response streamingly.
                            var text
                                in session.ChatAsync(
                                    new ChatHistory.Message(AuthorRole.User, this.userRequest),
                                    inferenceParams, cancellationToken))
                        {
                            OnTokenCallback(text);
                            sb.Append(text);
                        }
                    }
                    else
                    {
                        OnTokenCallback("Empty Request from User!");
                    }
                }

                if (OnResponseCompletedCallback != null)
                    OnResponseCompletedCallback(sb.ToString());
            }
        }

    }
}
