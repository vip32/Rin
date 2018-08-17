import { CheckboxVisibility, DetailsList, IColumn, Icon, SearchBox } from 'office-ui-fabric-react';
import * as React from 'react';
import { RequestEventPayload } from '../../api/IRinCoreHub';

export interface InspectorEventsListProps {
  onFilterChange: (newValue: string) => void;
  query: string;
  filteredItems: RequestEventPayload[];
  onActiveItemChanged: (item: RequestEventPayload) => void;
}

export class InspectorEventsList extends React.Component<InspectorEventsListProps> {
  private readonly columns: IColumn[] = [
    {
      key: 'icon',
      name: '',
      isIconOnly: true,
      minWidth: 16,
      maxWidth: 16,
      onRender: (item: RequestEventPayload) => {
        return (
          <Icon
            iconName={
              item.Path.match(/\.(jpg|png|svg)/)
                ? 'PictureCenter'
                : item.Path.match(/\.(js|vbs)/)
                  ? 'Script'
                  : item.Path.match(/\.(css)/)
                    ? 'FileCSS'
                    : item.Path.match(/\.html?/)
                      ? 'FileHTML'
                      : 'TextDocument'
            }
          />
        );
      }
    },
    {
      key: 'Path',
      name: 'Path',
      fieldName: 'Path',
      minWidth: 100,
      isResizable: true,
      onRender: (item: RequestEventPayload) => (
        <div
          title={item.Path}
          style={{ color: item.ResponseStatusCode >= 400 && item.ResponseStatusCode <= 599 ? '#a80000' : '#000' }}
        >
          <div className="inspectorEventsItem_Path">{item.Path}</div>
          <div className="inspectorEventsItem_ReceivedAt">{new Date(item.RequestReceivedAt).toLocaleString()}</div>
        </div>
      )
    },
    {
      key: 'ResponseStatusCode',
      name: 'StatusCode',
      fieldName: 'ResponseStatusCode',
      minWidth: 64,
      isResizable: true,
      onRender: (item: RequestEventPayload) =>
        item.ResponseStatusCode === 0 ? (
          <span>-</span>
        ) : (
          <span
            className="inspectorEventsItem_ResponseStatusCode"
            style={{ color: item.ResponseStatusCode >= 400 && item.ResponseStatusCode <= 599 ? '#a80000' : '' }}
          >
            {item.ResponseStatusCode}
          </span>
        )
    }
  ];

  render() {
    return (
      <>
        <SearchBox
          placeholder="Filter"
          underlined={true}
          onChange={this.props.onFilterChange}
          value={this.props.query}
        />
        <DetailsList
          compact={true}
          checkboxVisibility={CheckboxVisibility.hidden}
          columns={this.columns}
          items={this.props.filteredItems}
          onActiveItemChanged={this.props.onActiveItemChanged}
          selectionPreservedOnEmptyClick={true}
        />
      </>
    );
  }
}