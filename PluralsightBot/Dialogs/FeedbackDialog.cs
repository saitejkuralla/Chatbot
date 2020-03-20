using FinanceBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace FinanceBot.Dialogs
{

    public class FeedbackDialog: ComponentDialog
    {
        #region Variables
        private readonly BotStateService _botStateService;
        private readonly BotServices _botServices;
        #endregion

        public FeedbackDialog(string dialogId, BotStateService botStateService, BotServices botServices) : base(dialogId)
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            _botServices = botServices ?? throw new System.ArgumentNullException(nameof(botServices));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            // Create Waterfall Steps
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };

            // Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(FeedbackDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(FeedbackDialog)}.name"));

            // Set the starting Dialog
            InitialDialogId = $"{nameof(FeedbackDialog)}.mainFlow";
        }


        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
         
            var results = await _botServices.FeedbackQnA.GetAnswersAsync(stepContext.Context);
            if (results.Any())
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(results.First().Answer), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Sorry, could not find an answer in the Q and A system."), cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
